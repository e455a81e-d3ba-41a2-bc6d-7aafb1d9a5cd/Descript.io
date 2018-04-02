namespace Descriptio.Parser

open System
open System.Linq
open Descriptio.Parser.Core

module public MarkdownLexer =
    open System

    type public StackSymbols =
    | Z0
    | EmphasisStack
    | CodeBlockStart
    | CodeBlockStartLanguageChar of char

    type public State =
    | NewLine
    | TextLine
    | QuoteLine
    | TitleLevelState
    | TitleState
    | TitleClosingState
    | CodeBlock

    type public Token =
    | NewLineToken
    | EmphasisStartToken
    | EmphasisEndToken
    | CodeBlockStartToken
    | CodeBlockStartLanguageToken of language : string
    | TitleLevelToken
    | TitleToken of string
    | TitleClosingToken
    | TextToken of string

    type public Rule = (char list * State * StackSymbols list * Token list) -> (char list * State * StackSymbols list * Token list) option

    let inline (++) list tail = list@[tail]
    let inline (+!+) (list : 'a list) last = list.GetSlice(Some 0, Some(list.Length - 2))++last

    let (|LineBreak|_|) (input : char list) =
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
            fun (input, state, stack, output) -> match (input, state, stack) with ('#'::t, TitleClosingState, [Z0]) -> Some(t, TitleClosingState, stack, output++TitleClosingToken)| _ -> None;
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

    let emphasisRules : Rule list = [
            fun (input, state, stack, output) ->
                match (input, state, stack, output.LastOrDefault()) with
                | ('*'::'*'::t, NewLine, [Z0], _)
                | ('*'::'*'::t, TextLine, [Z0], _) -> Some(t, TextLine, EmphasisStack::stack, output++EmphasisStartToken)
                | ('*'::'*'::t, TextLine, EmphasisStack::s, _) -> Some(t, TextLine, s, output++EmphasisEndToken)
                | (LineBreak(t), TextLine, EmphasisStack::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + " "))
                | (c::t, TextLine, EmphasisStack::_, TextToken txt) -> Some(t, TextLine, stack, output+!+TextToken(txt + c.ToString()))
                | (c::t, TextLine, EmphasisStack::_, _) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))
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
            member __.Lex input = lexer ([for c in input -> c], NewLine, [Z0], [])
