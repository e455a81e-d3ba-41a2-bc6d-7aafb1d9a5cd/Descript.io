namespace Descriptio.Parser

open System
open System.Linq
open Descriptio.Core.AST
open Descriptio.Parser.Core
open Descriptio.Parser.MarkdownLexer

module public MarkdownParser =
    type ParserRule = Token seq -> (IAbstractSyntaxTree * Token seq) option
    
    let rec GetTitleLevel i currentRecDepth =
        match (i, currentRecDepth) with
        | (_, n) when n > 6 -> None
        | (TitleLevelToken::t, n) -> GetTitleLevel t (n + 1)
        | _ -> Some(i, currentRecDepth)
        

    let (|AtxTitleClosing|_|) input =
        let rec Pop i =
            match i with TitleClosingToken::t -> Pop t | _ -> i
        Some <| Pop input

    let atxTitleRules : ParserRule list = [
            fun input ->
                let titleLevelResult = GetTitleLevel (input |> Seq.toList) 0
                if titleLevelResult |> Option.isNone then None
                else match titleLevelResult.Value with
                     | (TitleToken(txt)::AtxTitleClosing(tail2), d) -> Some(TitleAst(txt, d) :> _, tail2 :> _)
                     | _ -> None
        ]

    let rules = atxTitleRules

    type public Parser() =
        member public this.Parse input = (this :> IParser<_>).Parse input
        interface IParser<Token seq> with
            member __.Parse input = raise <| NotImplementedException()