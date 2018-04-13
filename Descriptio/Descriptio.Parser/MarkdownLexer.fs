namespace Descriptio.Parser

open System
open System.Linq
open System.Runtime.CompilerServices
open System.Text
open Descriptio.Parser.Core

module public MarkdownLexer =

    type public State =
    | NewLine
    | TextLine
    | InlineCodeState
    | InlineCodeLiteralState
    | TitleLevelState
    | TitleState
    | TitleClosingState
    | ImageAltState
    | HyperlinkTextState
    | LinkState
    | LinkTitleState
    | EnumerationState
    | UnorderedEnumerationState
    | CodeBlockState

    type public StackSymbols =
    | Z0
    | TextStack of string
    | EmphasisStack of char
    | StrongStack of char
    | InlineCodeStack of State
    | ImageStack of State
    | HyperlinkStack of State
    | CodeBlockLanguageStack of string
    | CodeBlockContentStack of string

    type public Token =
    | NewLineToken
    | EmphasisStartToken
    | EmphasisEndToken
    | StrongStartToken
    | StrongEndToken
    | InlineCodeStartToken
    | InlineCodeEndToken
    | TitleLevelToken
    | TitleToken of string
    | TitleClosingToken
    | TextToken of string
    | ImageAltStartToken
    | ImageAltEndToken
    | LinkTextStartToken
    | LinkTextEndToken
    | LinkStartToken
    | LinkEndToken
    | EnumerationToken of int
    | UnorderedEnumerationToken of indent : int * char
    | CodeBlockStartToken
    | CodeBlockEndToken
    | CodeBlockLanguageToken of string

    [<Extension>]
    type public CharListExtensions =
        [<Extension>]
        static member inline BuildString(self: char list) = (self |> List.fold (fun (sb : StringBuilder) -> sb.Append) (StringBuilder())).ToString()


    type public Rule = (char list * State * StackSymbols list * Token list) -> (char list * State * StackSymbols list * Token list) option

    let inline (++) list tail = list@[tail]
    let inline (+!+) (list : 'a list) last = list.GetSlice(Some 0, Some(list.Length - 2))++last

    let CharSeqUntil delimiters =
        let rec charSeq delimiters currentSeq input =
            match (input, currentSeq) with
            | ([], []) -> None
            | (c::_, []) when delimiters |> List.contains c -> None
            | (c::_, _) when delimiters |> List.contains c -> Some(currentSeq, input)
            | ([], _) -> Some(currentSeq, [])
            | (c::t, _) -> charSeq delimiters (currentSeq++c) t
        
        charSeq delimiters [] 

    let (|ImgLinkTitle|_|) = CharSeqUntil ['"']
    let (|LinkHrefChars|_|) = CharSeqUntil [' '; ')']
    let (|ImgOrLinkStack|_|) stack = match stack with ImageStack(st)::t | HyperlinkStack(st)::t -> Some(st, t) | _ -> None

    
    let (|StrongSupportedStack|_|) stack =
        match stack with
        | Z0::st | TextStack(_)::st | EmphasisStack(_)::st -> Some(st)
        | _ -> None

    let (|IsUnorderedEnumerationToken|_|) input =
        match input with
        | '*'::t -> Some('*', t)
        | '+'::t -> Some('+', t)
        | '-'::t -> Some('-', t)
        | _ -> None
    
    let (|EnumIndent|_|) =
        let rec enumIndent level input =
            match input with
            | '\t'::t
            | ' '::' '::' '::' '::t -> enumIndent (level+1) t
            | _ -> Some(level, input)
        
        enumIndent 0

    let (|LineBreak|_|) input =
        match input with
        | '\r'::'\n'::t
        | '\r'::t
        | '\n'::t -> Some t
        | _ -> None

    let (|Escaped|_|) input = match input with '\\'::c::t -> Some(c, t) | _ -> None

    let (|CollapsedWhitespaces|_|) =
        let rec collapseWhitespace hasWhitespace input =
            match input with
            | ' '::t -> collapseWhitespace true t
            | _ when hasWhitespace -> Some(input)
            | _ -> None
        
        collapseWhitespace false

    let (|InlineSupportedState|_|) state =
        match state with
        | NewLine -> Some TextLine
        | TextLine
        | EnumerationState
        | UnorderedEnumerationState -> Some state
        | _ -> None

    let (|InlineState|_|) state =
        match state with
        | TextLine
        | EnumerationState
        | UnorderedEnumerationState -> Some state
        | _ -> None

    let (|IntNumber|_|) input =
        let rec GetIntNumber (input : char list) (digits : string) =
            match (input, digits) with
            | ([], "") -> None
            | ([], _) -> Some(digits |> Int32.Parse, [])
            | (delimiter::_, "") when delimiter |> Char.IsDigit |> not -> None
            | (delimiter::_, _) when delimiter |> Char.IsDigit |> not -> Some(digits |> Int32.Parse, input)
            | (digit::t, _) -> GetIntNumber t (digits + digit.ToString())
        
        GetIntNumber input ""

    let CodeBlockRules(input, state, stack, output) =
        match (input, state, stack) with
        | ('`'::'`'::'`'::LineBreak(t), NewLine, [Z0]) -> Some(t, CodeBlockState, stack, output++CodeBlockStartToken)
        | ('`'::'`'::'`'::t, NewLine, [Z0]) -> Some(t, CodeBlockState, CodeBlockLanguageStack("")::stack, output++CodeBlockStartToken)
        | (LineBreak('`'::'`'::'`'::LineBreak(t)), CodeBlockState, [CodeBlockContentStack(cnt); Z0]) -> Some(t, NewLine, [Z0], output@[TextToken(cnt); CodeBlockEndToken])
        | (LineBreak('`'::'`'::'`'::[]), CodeBlockState, [CodeBlockContentStack(cnt); Z0]) -> Some([], NewLine, [Z0], output@[TextToken(cnt); CodeBlockEndToken])
        | (c::t, CodeBlockState, [CodeBlockContentStack(cnt); Z0]) -> Some(t, CodeBlockState, [CodeBlockContentStack(cnt + c.ToString()); Z0], output)
        | (c::t, CodeBlockState, [Z0]) -> Some(t, CodeBlockState, CodeBlockContentStack(c.ToString())::stack, output)
        | (c::LineBreak(t), CodeBlockState, CodeBlockLanguageStack(lang)::s) -> Some(t, CodeBlockState, s, output++CodeBlockLanguageToken(String.Concat(lang, c.ToString())))
        | (c::t, CodeBlockState, CodeBlockLanguageStack(lang)::s) -> Some(t, CodeBlockState, CodeBlockLanguageStack(String.Concat(lang, c.ToString()))::s, output)
        | _ -> None

    let titleRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | ('#'::CollapsedWhitespaces(t), NewLine, [Z0]) -> Some(t, TitleState, stack, output++TitleLevelToken)
                | ('#'::CollapsedWhitespaces(t), TitleLevelState, [Z0]) -> Some(t, TitleState, stack, output++TitleLevelToken)
                | (LineBreak('#'::t), TextLine, [Z0])
                | ('#'::t, NewLine, [Z0])
                | ('#'::t, TitleLevelState, [Z0]) -> Some(t, TitleLevelState, stack, output++TitleLevelToken)
                | _ -> None;

            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (CollapsedWhitespaces('#'::t), TitleState, TextStack(txt)::s)
                | ('#'::t, TitleState, TextStack(txt)::s) -> Some(t, TitleClosingState, s, output@[TitleToken(txt);TitleClosingToken])
                | ('#'::t, TitleClosingState, [Z0]) -> Some(t, TitleClosingState, stack, output++TitleClosingToken)
                | _ -> None;

            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (LineBreak(t), TitleState, TextStack(txt)::s) -> Some(t, NewLine, s, output@[TitleToken(txt);NewLineToken])
                | (LineBreak(t), TitleClosingState, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | ([], TitleState, TextStack(txt)::s) -> Some([], NewLine, s, output++TitleToken(txt))
                | ([], TitleClosingState, [Z0]) -> Some([], NewLine, stack, output)
                | _ -> None;
                
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (c::t, TitleState, TextStack(txt)::s) -> Some(t, TitleState, TextStack(txt + c.ToString())::s, output)
                | (c::t, TitleState, [Z0]) -> Some(t, TitleState, TextStack(c.ToString())::stack, output)
                | _ -> None;
        ]

    let unorderedEnumerationRules(input, state, stack, output) =
        match (input, state, stack) with
        | (EnumIndent(indent, IsUnorderedEnumerationToken(chr, ' '::t)), NewLine, [Z0]) -> Some(t, UnorderedEnumerationState, stack, output++UnorderedEnumerationToken(indent, chr))
        | (LineBreak(EnumIndent(indent, IsUnorderedEnumerationToken(chr, ' '::t))), UnorderedEnumerationState, [Z0]) -> Some(t, UnorderedEnumerationState, stack, output++UnorderedEnumerationToken(indent, chr))
        | (LineBreak(LineBreak(t)), UnorderedEnumerationState, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
        | _ -> None;
    
    let enumerationRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (IntNumber(num, '.'::CollapsedWhitespaces(t)), NewLine, [Z0])
                | (LineBreak(IntNumber(num, '.'::CollapsedWhitespaces(t))), EnumerationState, [Z0]) -> Some(t, EnumerationState, stack, output++EnumerationToken(num))
                | (LineBreak(LineBreak(t)), EnumerationState, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | _ -> None;
        ]

    let blockRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (LineBreak(LineBreak t), TextLine, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | (LineBreak(LineBreak t), NewLine, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | _ -> None;
        ]

    let imageHyperlinkRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | ('!'::'['::t, InlineSupportedState st, [Z0]) -> Some(t, ImageAltState, ImageStack(st)::stack, output++ImageAltStartToken)

                | ('['::t, InlineSupportedState st, [Z0]) -> Some(t, HyperlinkTextState, HyperlinkStack(st)::stack, output++LinkTextStartToken)

                | (']'::'('::t, ImageAltState, TextStack(txt)::s) -> Some(t, LinkState, s, output++TextToken(txt)++ImageAltEndToken++LinkStartToken)
                | (']'::'('::t, HyperlinkTextState, TextStack(txt)::s) -> Some(t, LinkState, s, output++TextToken(txt)++LinkTextEndToken++LinkStartToken)

                | (c::t, ImageAltState, TextStack(txt)::ImageStack(st)::s) -> Some(t, state, TextStack(txt + c.ToString())::ImageStack(st)::s, output)
                | (c::t, HyperlinkTextState, TextStack(txt)::HyperlinkStack(st)::s) -> Some(t, state, TextStack(txt + c.ToString())::HyperlinkStack(st)::s, output)
                | (c::t, ImageAltState, ImageStack(_)::_)
                | (c::t, HyperlinkTextState, HyperlinkStack(_)::_) -> Some(t, state, TextStack(c.ToString())::stack, output)
 
                | ('"'::')'::t, LinkTitleState, TextStack(txt)::ImageStack(oldState)::st)
                | (')'::t, LinkState, TextStack(txt)::ImageStack(oldState)::st)
                | ('"'::')'::t, LinkTitleState, TextStack(txt)::HyperlinkStack(oldState)::st)
                | (')'::t, LinkState, TextStack(txt)::HyperlinkStack(oldState)::st) -> Some(t, oldState, st, output++TextToken(txt)++LinkEndToken)

                | (LinkHrefChars(txt, t), LinkState, ImgOrLinkStack(_)) -> Some(t, state, TextStack(txt.BuildString())::stack, output)
                | (ImgLinkTitle(txt, t), LinkTitleState, ImgOrLinkStack(_)) -> Some(t, state, TextStack(txt.BuildString())::stack, output)
                | (' '::'"'::t, LinkState, TextStack(txt)::s) -> Some(t, LinkTitleState, s, output++TextToken(txt))

                | _ -> None;
        ]

    let inlineCodeRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | ('`'::'`'::t, InlineSupportedState st, [Z0]) -> Some(t, InlineCodeLiteralState, InlineCodeStack(st)::stack, output++InlineCodeStartToken)
                | ('`'::'`'::t, InlineCodeLiteralState, TextStack(txt)::InlineCodeStack(st)::s) -> Some(t, st, s, output++TextToken(txt)++InlineCodeEndToken)
                | (LineBreak(t), InlineCodeLiteralState, TextStack(txt)::s) -> Some(t, InlineCodeLiteralState, TextStack(txt + " ")::s, output)
                | (LineBreak(t), InlineCodeState, TextStack(txt)::s) -> Some(t, InlineCodeState, TextStack(txt + " ")::s, output)

                | (c::t, InlineCodeLiteralState, TextStack(txt)::s) -> Some(t, InlineCodeLiteralState, TextStack(txt + c.ToString())::s, output)
                | (c::t, InlineCodeLiteralState, InlineCodeStack(_)::_) -> Some(t, InlineCodeLiteralState, TextStack(c.ToString())::stack, output)
                | ('`'::t, InlineSupportedState st, [Z0]) -> Some(t, InlineCodeState, InlineCodeStack(st)::stack, output++InlineCodeStartToken)
                | ('`'::t, InlineCodeState, TextStack(txt)::InlineCodeStack(st)::s) -> Some(t, st, s, output++TextToken(txt)++InlineCodeEndToken)
                | (LineBreak(t), InlineCodeState, TextStack(txt)::s) -> Some(t, state, TextStack(txt + " ")::s, output)
                | (c::t, InlineCodeState, TextStack(txt)::s) -> Some(t, state, TextStack(txt + c.ToString())::s, output)
                | (c::t, InlineCodeState, InlineCodeStack(_)::_) -> Some(t, state, TextStack(c.ToString())::stack, output)
                | _ -> None;
        ]

    let strongRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (Escaped('_', _), InlineSupportedState _, TextStack(_)::_)
                | (Escaped('*', _), InlineSupportedState _, TextStack(_)::_)
                | ('*'::' '::_, InlineSupportedState _, TextStack(_)::_)
                | ('_'::' '::_, InlineSupportedState _, TextStack(_)::_)
                | (' '::'*'::_, InlineSupportedState _, TextStack(_)::_) 
                | (' '::'_'::_, InlineSupportedState _, TextStack(_)::_) -> None
                | ('*'::'*'::t, InlineSupportedState st, [Z0]) -> Some(t, st, StrongStack('*')::stack, output++StrongStartToken)
                | ('*'::'*'::t, InlineSupportedState st, TextStack(txt)::StrongStack('*')::s) -> Some(t, st, s, output++TextToken(txt)++StrongEndToken)
                | ('_'::'_'::t, InlineSupportedState st, [Z0]) -> Some(t, st, StrongStack('_')::stack, output++StrongStartToken)
                | ('_'::'_'::t, InlineSupportedState st, StrongStack('_')::s) -> Some(t, st, s, output++StrongEndToken)
                | (LineBreak(t), InlineSupportedState st, TextStack(txt)::StrongStack(_)::_) -> Some(t, st, TextStack(txt + " ")::stack.Tail, output)
                | (c::t, InlineSupportedState st, TextStack(txt)::StrongStack(_)::_) -> Some(t, st, TextStack(txt + c.ToString())::stack.Tail, output)
                | (c::t, InlineSupportedState st, StrongStack(_)::_) -> Some(t, st, TextStack(c.ToString())::stack, output)
                | _ -> None;
        ]

    let emphasisRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('*'::t, InlineSupportedState st, [Z0], _) -> Some(t, st, EmphasisStack('*')::stack, output++EmphasisStartToken)
                | ('*'::t, InlineSupportedState st, EmphasisStack('*')::s, _) -> Some(t, st, s, output++EmphasisEndToken)
                | ('_'::t, InlineSupportedState st, [Z0], _) -> Some(t, st, EmphasisStack('_')::stack, output++EmphasisStartToken)
                | ('_'::t, InlineSupportedState st, EmphasisStack('_')::s, _) -> Some(t, st, s, output++EmphasisEndToken)
                | (LineBreak(t), InlineSupportedState st, EmphasisStack(_)::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + " "))
                | (c::t, InlineSupportedState st, EmphasisStack(_)::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, InlineSupportedState st, EmphasisStack(_)::_, _) -> Some(t, st, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]
    
    let textLineRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output |> List.tryLast) with
                | (CollapsedWhitespaces(t), NewLine, [Z0], _) -> Some(t, state, [Z0], output)

                | (CollapsedWhitespaces(LineBreak(t)), InlineState st, [Z0], Some(TextToken txt))
                | (CollapsedWhitespaces(t), InlineState st, [Z0], Some(TextToken txt)) -> Some(t, st, [Z0], output+!+TextToken(txt + " "))

                | (LineBreak(CollapsedWhitespaces(t)), TextLine, [Z0], Some(TextToken txt))
                | (LineBreak(t), TextLine, [Z0], Some(TextToken txt)) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, NewLine, [Z0], _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | (c::t, InlineSupportedState st, [Z0], Some(TextToken txt)) -> Some(t, st, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, InlineSupportedState st, [Z0], _) -> Some(t, st, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]

    let rules : Rule list =
        textLineRules
        |> List.append emphasisRules
        |> List.append strongRules
        |> List.append inlineCodeRules
        |> List.append imageHyperlinkRules
        |> List.append blockRules
        |> List.append (enumerationRules++unorderedEnumerationRules)
        |> List.append (titleRules++CodeBlockRules)

    type public TextLexer() =
        let rec lexer (inp, state, stack, output) =
            rules
            |> Seq.map (fun r -> r(inp, state, stack, output))
            |> Seq.filter Option.isSome
            |> Seq.map (fun r -> match r.Value with
                                  | ([], _, [Z0], _) -> Some(r.Value |> TupleExtensions.ToValueTuple)
                                  | _ -> lexer r.Value)
            |> Seq.tryPick (fun s -> s)
        
        member public this.Lex input = (this :> ILexer<_, _>).Lex input
        interface ILexer<string, struct(char list * State * StackSymbols list * Token list)> with
            member __.Lex input = lexer (input.ToCharArray() |> Array.toList, NewLine, [Z0], [])
