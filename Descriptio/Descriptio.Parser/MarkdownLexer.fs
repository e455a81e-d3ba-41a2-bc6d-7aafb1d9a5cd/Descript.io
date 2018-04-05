namespace Descriptio.Parser

open System
open System.Linq
open Descriptio.Parser.Core

module public MarkdownLexer =

    type public StackSymbols =
    | Z0
    | EmphasisStack of char
    | StrongStack of char
    | InlineCodeStack
    | InlineCodeLiteralStack
    | ImageStack
    | HyperlinkStack

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


    type public Rule = (char list * State * StackSymbols list * Token list) -> (char list * State * StackSymbols list * Token list) option

    let inline (++) list tail = list@[tail]
    let inline (+!+) (list : 'a list) last = list.GetSlice(Some 0, Some(list.Length - 2))++last

    let (|LineBreak|_|) input =
        match input with
        | '\r'::'\n'::t
        | '\r'::t
        | '\n'::t -> Some t
        | _ -> None

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

    let blockRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack) with
                | (LineBreak(LineBreak t), TextLine, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | (LineBreak(LineBreak t), NewLine, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)
                | _ -> None
        ]

    let imageHyperlinkRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('!'::'['::t, TextLine, [Z0], _)
                | ('!'::'['::t, NewLine, [Z0], _) -> Some(t, ImageAltState, ImageStack::stack, output++ImageAltStartToken)

                | ('['::t, TextLine, [Z0], _)
                | ('['::t, NewLine, [Z0], _) -> Some(t, HyperlinkTextState, HyperlinkStack::stack, output++LinkTextStartToken)

                | (']'::'('::t, ImageAltState, ImageStack::_, _) -> Some(t, LinkState, stack, output++ImageAltEndToken++LinkStartToken)
                | (']'::'('::t, HyperlinkTextState, HyperlinkStack::_, _) -> Some(t, LinkState, stack, output++LinkTextEndToken++LinkStartToken)

                | (c::t, ImageAltState, ImageStack::_, TextToken(txt))
                | (c::t, HyperlinkTextState, HyperlinkStack::_, TextToken(txt)) -> Some(t, state, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, ImageAltState, ImageStack::_, _)
                | (c::t, HyperlinkTextState, HyperlinkStack::_, _) -> Some(t, state, stack, output++TextToken(c.ToString()))
 
                | ('"'::')'::t, LinkTitleState, ImageStack::st, _)
                | (')'::t, LinkState, ImageStack::st, _)
                | ('"'::')'::t, LinkTitleState, HyperlinkStack::st, _)
                | (')'::t, LinkState, HyperlinkStack::st, _) -> Some(t, TextLine, st, output++LinkEndToken)

                | (' '::'"'::t, LinkState, ImageStack::_, _) -> Some(t, LinkTitleState, stack, output++TextToken(""))
                | (' '::'"'::t, LinkState, HyperlinkStack::_, _) -> Some(t, LinkTitleState, stack, output++TextToken(""))
                | (c::t, LinkState, ImageStack::_, TextToken(txt))
                | (c::t, LinkState, HyperlinkStack::_, TextToken(txt)) -> Some(t, LinkState, stack, output+!+TextToken(txt + c.ToString()))

                | (c::t, LinkState, ImageStack::_, _)
                | (c::t, LinkState, HyperlinkStack::_, _) -> Some(t, LinkState, stack, output++TextToken(c.ToString()))

                | (c::t, LinkTitleState, ImageStack::_, TextToken(txt))
                | (c::t, LinkTitleState, HyperlinkStack::_, TextToken(txt)) -> Some(t, LinkTitleState, stack, output+!+TextToken(txt + c.ToString()))

                | (c::t, LinkTitleState, ImageStack::_, _)
                | (c::t, LinkTitleState, HyperlinkStack::_, _) -> Some(t, LinkTitleState, stack, output++TextToken(c.ToString()))

                | _ -> None;
        ]

    let inlineCodeRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('`'::'`'::t, NewLine, [Z0], _)
                | ('`'::'`'::t, TextLine, [Z0], _) -> Some(t, TextLine, InlineCodeLiteralStack::stack, output++InlineCodeStartToken)
                | ('`'::'`'::t, TextLine, InlineCodeLiteralStack::s, _) -> Some(t, TextLine, s, output++InlineCodeEndToken)
                | (LineBreak(t), TextLine, InlineCodeLiteralStack::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, TextLine, InlineCodeLiteralStack::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, TextLine, InlineCodeLiteralStack::_, _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | ('`'::t, NewLine, [Z0], _)
                | ('`'::t, TextLine, [Z0], _) -> Some(t, TextLine, InlineCodeStack::stack, output++InlineCodeStartToken)
                | ('`'::t, TextLine, InlineCodeStack::s, _) -> Some(t, TextLine, s, output++InlineCodeEndToken)
                | (LineBreak(t), TextLine, InlineCodeStack::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, TextLine, InlineCodeStack::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, TextLine, InlineCodeStack::_, _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]

    let strongRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('\\'::'*'::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + "*"))
                | ('*'::' '::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + "* "))
                | (' '::'*'::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " *"))
                | ('\\'::'_'::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + "_"))
                | ('_'::' '::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + "* "))
                | (' '::'_'::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " *"))
                | ('*'::'*'::t, NewLine, [Z0], _)
                | ('*'::'*'::t, TextLine, [Z0], _) -> Some(t, TextLine, StrongStack('*')::stack, output++StrongStartToken)
                | ('*'::'*'::t, TextLine, StrongStack('*')::s, _) -> Some(t, TextLine, s, output++StrongEndToken)
                | ('_'::'_'::t, NewLine, [Z0], _)
                | ('_'::'_'::t, TextLine, [Z0], _) -> Some(t, TextLine, StrongStack('_')::stack, output++StrongStartToken)
                | ('_'::'_'::t, TextLine, StrongStack('_')::s, _) -> Some(t, TextLine, s, output++StrongEndToken)
                | (LineBreak(t), TextLine, StrongStack(_)::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, TextLine, StrongStack(_)::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, TextLine, StrongStack(_)::_, _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]

    let emphasisRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('*'::t, NewLine, [Z0], _)
                | ('*'::t, TextLine, [Z0], _) -> Some(t, TextLine, EmphasisStack('*')::stack, output++EmphasisStartToken)
                | ('*'::t, TextLine, EmphasisStack('*')::s, _) -> Some(t, TextLine, s, output++EmphasisEndToken)
                | ('_'::t, NewLine, [Z0], _)
                | ('_'::t, TextLine, [Z0], _) -> Some(t, TextLine, EmphasisStack('_')::stack, output++EmphasisStartToken)
                | ('_'::t, TextLine, EmphasisStack('_')::s, _) -> Some(t, TextLine, s, output++EmphasisEndToken)
                | (LineBreak(t), TextLine, EmphasisStack(_)::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, TextLine, EmphasisStack(_)::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, TextLine, EmphasisStack(_)::_, _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]
    
    let textLineRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | (LineBreak(t), TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, NewLine, [Z0], _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | (c::t, TextLine, [Z0], TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, TextLine, [Z0], _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
                | _ -> None;
        ]

    let rules : Rule list =
        textLineRules
        |> List.append emphasisRules
        |> List.append strongRules
        |> List.append inlineCodeRules
        |> List.append imageHyperlinkRules
        |> List.append blockRules
        |> List.append titleRules

    type public TextLexer() =
        let rec lexer (inp, state, stack, output) =
            rules
            |> List.map (fun r -> r(inp, state, stack, output))
            |> List.filter Option.isSome
            |> List.map (fun r -> match r.Value with
                                  | ([], _, [Z0], _) -> Some(r.Value |> TupleExtensions.ToValueTuple)
                                  | _ -> lexer r.Value)
            |> List.fold (fun s res -> if Option.isSome s then s else res) None
        
        member public this.Lex input = (this :> ILexer<_, _>).Lex input
        interface ILexer<string, struct(char list * State * StackSymbols list * Token list)> with
            member __.Lex input = lexer (input.ToCharArray() |> Array.toList, NewLine, [Z0], [])
