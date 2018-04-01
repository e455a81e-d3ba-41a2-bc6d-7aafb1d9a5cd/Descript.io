namespace Descriptio.Parser

open System
open System.Linq
open Descriptio.Parser.Core

module public Markdown =

    type public StackSymbols = Z0 | EmphasisStack | CodeBlockStart | CodeBlockStartLanguageChar of char

    type public State =
    | NewLine
    | TextLine
    | QuoteLine
    | CodeBlock

    type public Token =
    | NewLineToken
    | EmphasisStartToken
    | EmphasisEndToken
    | CodeBlockStartToken
    | CodeBlockStartLanguageToken of language : string
    | TextToken of string

    type public Rule = (char list * State * StackSymbols list * Token list) -> (char list * State * StackSymbols list * Token list) option

    let (++) list tail = List.append list [tail]

    let rules : Rule list = [
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('\r'::'\n'::t, TextLine, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)| _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('*'::'*'::t, TextLine, [Z0]) -> Some(t, TextLine, EmphasisStack::stack, output++EmphasisStartToken)| _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('*'::'*'::t, TextLine, EmphasisStack::s) -> Some(t, TextLine, s, output++EmphasisEndToken)| _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('`'::'`'::'`'::t, NewLine, [Z0]) -> Some(t, CodeBlock, CodeBlockStart::stack, output++CodeBlockStartToken) | _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack, output.Last()) with ('\r'::'\n'::_, CodeBlock, CodeBlockStartLanguageChar(c)::st, CodeBlockStartLanguageToken(l)) -> Some(input, CodeBlock, st, output.GetSlice(Some 0, Some <| output.Length - 2)++CodeBlockStartLanguageToken(l + c.ToString())) | _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack, output) with ('\r'::'\n'::_, CodeBlock, CodeBlockStartLanguageChar(c)::st, _) -> Some(input, CodeBlock, st, output++CodeBlockStartLanguageToken(c.ToString())) | _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with (h::t, CodeBlock, CodeBlockStart::_) -> Some(t, CodeBlock, CodeBlockStartLanguageChar(h)::stack, output) | _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with (c::t, TextLine, [Z0]) -> Some(t, TextLine, stack, output++TextToken(c.ToString()))| _ -> None));
        ]

    type public TextLexer() =
        let rec lexer (inp, state, stack, output) =
            rules
            |> List.map (fun r -> r(inp, state, stack, output))
            |> List.filter Option.isSome
            |> List.map (fun r1 -> r1.Value)
            |> List.map lexer
            |> List.fold (fun s res -> if Option.isSome res then res else s) None
        
        member public this.Lex input = (this :> ILexer<_, _>).Lex input
        interface ILexer<string, (char list * State * StackSymbols list * Token list)> with
            member __.Lex input = lexer ([for c in input -> c], NewLine, [Z0], [])

    type public TextParser() =
        member public this.Parse input = (this :> IParser<_>).Parse input
        interface IParser<(char list * State * StackSymbols list * Token list)> with
            member __.Parse input = raise <| NotImplementedException()
