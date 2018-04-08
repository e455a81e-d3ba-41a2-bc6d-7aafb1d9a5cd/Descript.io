namespace Descriptio.Parser

open System
open System.Linq
open Descriptio.Parser.Core

module public MarkdownLexer =

    type public State =
    | NewLine
    | TextLine
    | TitleLevelState
    | TitleState
    | TitleClosingState
    | ImageAltState
    | HyperlinkTextState
    | LinkState
    | LinkTitleState
    | EnumerationState
    | UnorderedEnumerationState

    type public StackSymbols =
    | Z0
    | EmphasisStack of char
    | StrongStack of char
    | InlineCodeStack
    | InlineCodeLiteralStack
    | ImageStack of State
    | HyperlinkStack of State

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


    type public Rule = (char list * State * StackSymbols list * Token list) -> (char list * State * StackSymbols list * Token list) option

    let inline (++) list tail = list@[tail]
    let inline (+!+) (list : 'a list) last = list.GetSlice(Some 0, Some(list.Length - 2))++last

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

    let titleRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('#'::t, TextLine, [Z0], NewLineToken)
                | ('#'::t, NewLine, [Z0], _)
                | ('#'::t, TitleLevelState, [Z0], _) -> Some(t, TitleLevelState, stack, output++TitleLevelToken)
                | (' '::t, TitleLevelState, [Z0], _) -> Some(t, TitleState, stack, output)
                | _ -> None;

            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (' '::'#'::t, TitleState, [Z0])
                | ('#'::t, TitleState, [Z0]) -> Some(t, TitleClosingState, stack, output++TitleClosingToken)
                | _ -> None;
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | ('#'::t, TitleClosingState, [Z0]) -> Some(t, TitleClosingState, stack, output++TitleClosingToken)
                | _ -> None;
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (LineBreak(t), TitleState, [Z0])
                | (LineBreak(t), TitleClosingState, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | _ -> None;    
                
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | (c::t, TitleState, [Z0], TitleToken tk) -> Some(t, TitleState, stack, output+!+TitleToken(tk + c.ToString()))
                | (c::t, TitleState, [Z0], TitleLevelToken) -> Some(t, TitleState, stack, output++TitleToken(c.ToString()))
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
                | (IntNumber(num, '.'::' '::t), NewLine, [Z0])
                | (LineBreak(IntNumber(num, '.'::' '::t)), EnumerationState, [Z0]) -> Some(t, EnumerationState, stack, output++EnumerationToken(num))
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
                match (input, state, stack, output.LastOrDefault()) with
                | ('!'::'['::t, InlineSupportedState st, [Z0], _) -> Some(t, ImageAltState, ImageStack(st)::stack, output++ImageAltStartToken)

                | ('['::t, InlineSupportedState st, [Z0], _) -> Some(t, HyperlinkTextState, HyperlinkStack(st)::stack, output++LinkTextStartToken)

                | (']'::'('::t, ImageAltState, ImageStack(_)::_, _) -> Some(t, LinkState, stack, output++ImageAltEndToken++LinkStartToken)
                | (']'::'('::t, HyperlinkTextState, HyperlinkStack(_)::_, _) -> Some(t, LinkState, stack, output++LinkTextEndToken++LinkStartToken)

                | (c::t, ImageAltState, ImageStack(_)::_, TextToken(txt))
                | (c::t, HyperlinkTextState, HyperlinkStack(_)::_, TextToken(txt)) -> Some(t, state, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, ImageAltState, ImageStack(_)::_, _)
                | (c::t, HyperlinkTextState, HyperlinkStack(_)::_, _) -> Some(t, state, stack, output++TextToken(c.ToString()))
 
                | ('"'::')'::t, LinkTitleState, ImageStack(oldState)::st, _)
                | (')'::t, LinkState, ImageStack(oldState)::st, _)
                | ('"'::')'::t, LinkTitleState, HyperlinkStack(oldState)::st, _)
                | (')'::t, LinkState, HyperlinkStack(oldState)::st, _) -> Some(t, oldState, st, output++LinkEndToken)

                | (' '::'"'::t, LinkState, ImageStack(_)::_, _) -> Some(t, LinkTitleState, stack, output++TextToken(""))
                | (' '::'"'::t, LinkState, HyperlinkStack(_)::_, _) -> Some(t, LinkTitleState, stack, output++TextToken(""))
                | (c::t, LinkState, ImageStack(_)::_, TextToken(txt))
                | (c::t, LinkState, HyperlinkStack(_)::_, TextToken(txt)) -> Some(t, LinkState, stack, output+!+TextToken(txt + c.ToString()))

                | (c::t, LinkState, ImageStack(_)::_, _)
                | (c::t, LinkState, HyperlinkStack(_)::_, _) -> Some(t, LinkState, stack, output++TextToken(c.ToString()))

                | (c::t, LinkTitleState, ImageStack(_)::_, TextToken(txt))
                | (c::t, LinkTitleState, HyperlinkStack(_)::_, TextToken(txt)) -> Some(t, LinkTitleState, stack, output+!+TextToken(txt + c.ToString()))

                | (c::t, LinkTitleState, ImageStack(_)::_, _)
                | (c::t, LinkTitleState, HyperlinkStack(_)::_, _) -> Some(t, LinkTitleState, stack, output++TextToken(c.ToString()))

                | _ -> None;
        ]

    let inlineCodeRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('`'::'`'::t, InlineSupportedState st, [Z0], _) -> Some(t, st, InlineCodeLiteralStack::stack, output++InlineCodeStartToken)
                | ('`'::'`'::t, InlineSupportedState st, InlineCodeLiteralStack::s, _) -> Some(t, st, s, output++InlineCodeEndToken)
                | (LineBreak(t), InlineSupportedState st, InlineCodeLiteralStack::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + " "))
                | (c::t, InlineSupportedState st, InlineCodeLiteralStack::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, InlineSupportedState st, InlineCodeLiteralStack::_, _) -> Some(t, st, stack, output++TextToken(c.ToString()))
                | ('`'::t, InlineSupportedState st, [Z0], _) -> Some(t, st, InlineCodeStack::stack, output++InlineCodeStartToken)
                | ('`'::t, InlineSupportedState st, InlineCodeStack::s, _) -> Some(t, st, s, output++InlineCodeEndToken)
                | (LineBreak(t), InlineSupportedState st, InlineCodeStack::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + " "))
                | (c::t, InlineSupportedState st, InlineCodeStack::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, InlineSupportedState st, InlineCodeStack::_, _) -> Some(t, st, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]

    let strongRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('\\'::'*'::t, InlineSupportedState st, [Z0], TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + "*"))
                | ('*'::' '::t, InlineSupportedState st, [Z0], TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + "* "))
                | (' '::'*'::t, InlineSupportedState st, [Z0], TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + " *"))
                | ('\\'::'_'::t, InlineSupportedState st, [Z0], TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + "_"))
                | ('_'::' '::t, InlineSupportedState st, [Z0], TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + "* "))
                | (' '::'_'::t, InlineSupportedState st, [Z0], TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + " *"))
                | ('*'::'*'::t, InlineSupportedState st, [Z0], _) -> Some(t, st, StrongStack('*')::stack, output++StrongStartToken)
                | ('*'::'*'::t, InlineSupportedState st, StrongStack('*')::s, _) -> Some(t, st, s, output++StrongEndToken)
                | ('_'::'_'::t, InlineSupportedState st, [Z0], _) -> Some(t, st, StrongStack('_')::stack, output++StrongStartToken)
                | ('_'::'_'::t, InlineSupportedState st, StrongStack('_')::s, _) -> Some(t, st, s, output++StrongEndToken)
                | (LineBreak(t), InlineSupportedState st, StrongStack(_)::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + " "))
                | (c::t, InlineSupportedState st, StrongStack(_)::_, TextToken txt) -> Some(t, st, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, InlineSupportedState st, StrongStack(_)::_, _) -> Some(t, st, stack, output++TextToken(c.ToString()))
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
        |> List.append titleRules

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
