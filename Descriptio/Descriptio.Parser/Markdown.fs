namespace Descriptio.Parser

open System.IO
open Core
open Descriptio.Core.AST

module Markdown =
    open System

    type public StringParser =
        interface IParser<string> with
            member this.Parse source =
                raise <| NotImplementedException()
    
    type StackSymbols = Z0 | EmphasisStack

    type State =
    | NewLine
    | TextLine

    type Token =
    | NewLineToken
    | EmphasisStartToken
    | EmphasisEndToken
    | CharToken of char

    type Rule = (char list * State * StackSymbols list * Token list) -> (char list * State * StackSymbols list * Token list) option

    let (++) list tail = List.append list [tail]

    type public StreamParser() =
        let rules : Rule list = [
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('\r'::'\n'::t, TextLine, [Z0]) -> Some(t, NewLine, stack, output++NewLineToken)| _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('*'::'*'::t, TextLine, [Z0]) -> Some(t, TextLine, EmphasisStack::stack, output++EmphasisStartToken)| _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with ('*'::'*'::t, TextLine, EmphasisStack::s) -> Some(t, TextLine, s, output++EmphasisEndToken)| _ -> None));
            (fun (input, state, stack, output) -> (match (input, state, stack) with (c::t, TextLine, [Z0]) -> Some(t, TextLine, stack, output++CharToken c)| _ -> None))
        ]
