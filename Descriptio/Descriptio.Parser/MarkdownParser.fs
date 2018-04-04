namespace Descriptio.Parser

open System
open System.Linq
open Descriptio.Core.AST
open Descriptio.Parser.Core
open Descriptio.Parser.MarkdownLexer

module public MarkdownParser =
    let ParseAtxTitle input =
        let rec ParseTitleLevelTokens inp recDepth =
            match (inp, recDepth) with
            | (_, n) when n <= 0 || n > 6 -> None
            | (TitleLevelToken::TitleToken(txt)::t, _) -> Some(TitleToken(txt)::t, recDepth)
            | (TitleLevelToken::t, _) -> ParseTitleLevelTokens t (recDepth + 1)
            | _ -> None
        
        let ParseTitleToken inp =
            match inp with
            | Some(TitleToken(txt)::t, titleLevel) -> Some(TitleAst(txt, titleLevel) :> IAbstractSyntaxTreeBlock, t)
            | _ -> None

        let rec ParseTitleClosingTokens inp =
            match inp with
            | Some(title, TitleClosingToken::t) -> ParseTitleClosingTokens <| Some(title, t)
            | None -> None
            | Some(_, []) -> inp
            | Some(title, NewLineToken::t) -> Some(title, t)
            | _ -> None

        (input, 1) ||> ParseTitleLevelTokens |> ParseTitleToken |> ParseTitleClosingTokens
    
    let ParseTextParagraph input =
        let ParseInline input =
            let ParseCleanText inp =
                match inp with
                | TextToken(txt)::NewLineToken::TextToken(txt2)::t -> Some(CleanTextInline(txt + " " + txt2) :> IAbstractSyntaxTreeInline, t)
                | TextToken(txt)::t -> Some(CleanTextInline(txt) :> IAbstractSyntaxTreeInline, t)
                | _ -> None
        
            let ParseEmphasis inp =
                let ParseEmphasisStart i =
                    match i with
                    | EmphasisStartToken::t -> Some t
                    | _ -> None

                let ParseEmphasisText i =
                    match i with
                    | Some(TextToken(txt)::NewLineToken::TextToken(txt2)::t) -> Some(t, txt + " " + txt2)
                    | Some(TextToken(txt)::t) -> Some(t, txt)
                    | _ -> None
            
                let ParseEmphasisEnd par =
                    match par with
                    | Some(EmphasisEndToken::t, txt) -> Some(EmphasisTextInline(txt) :> IAbstractSyntaxTreeInline, t)
                    | _ -> None
            
                ParseEmphasisStart inp |> ParseEmphasisText |> ParseEmphasisEnd
        
            let ParseStrong inp =
                let ParseStrongStart i =
                    match i with
                    | StrongStartToken::t -> Some t
                    | _ -> None

                let ParseStrongText i =
                    match i with
                    | Some(TextToken(txt)::NewLineToken::TextToken(txt2)::t) -> Some(t, txt + " " + txt2)
                    | Some(TextToken(txt)::t) -> Some(t, txt)
                    | _ -> None
            
                let ParseStrongEnd par =
                    match par with
                    | Some(StrongEndToken::t, txt) -> Some(StrongTextInline(txt) :> IAbstractSyntaxTreeInline, t)
                    | _ -> None
            
                inp |> ParseStrongStart |> ParseStrongText |> ParseStrongEnd
        
            let ParseInlineCode inp =
                let ParseInlineCodeStart i =
                    match i with
                    | InlineCodeStartToken::t -> Some t
                    | _ -> None

                let ParseInlineCodeText i =
                    match i with
                    | Some(TextToken(txt)::NewLineToken::TextToken(txt2)::t) -> Some(t, txt + " " + txt2)
                    | Some(TextToken(txt)::t) -> Some(t, txt)
                    | _ -> None
            
                let ParseInlineCodeEnd par =
                    match par with
                    | Some(InlineCodeEndToken::t, txt) -> Some(CodeTextInline(txt) :> IAbstractSyntaxTreeInline, t)
                    | _ -> None
            
                inp |> ParseInlineCodeStart |> ParseInlineCodeText |> ParseInlineCodeEnd
        
            let ParseImageInline inp =
                let ParseImageInlineAlt i =
                    match i with
                    | ImageAltStartToken::TextToken(alt)::ImageAltEndToken::t -> Some(t, alt)
                    | _ -> None

                let ParseImageSrcTitleText i =
                    match i with
                    | Some(LinkStartToken::TextToken(src)::TextToken(title)::LinkEndToken::t, alt) ->
                        Some(ImageInline(alt, src, title) :> IAbstractSyntaxTreeInline, t)
                    | Some(LinkStartToken::TextToken(src)::LinkEndToken::t, alt) ->
                        Some(ImageInline(alt, src) :> IAbstractSyntaxTreeInline, t)
                    | _ -> None

                inp |> ParseImageInlineAlt |> ParseImageSrcTitleText

            let textParagraphRules = [
                    ParseCleanText;
                    ParseStrong;
                    ParseEmphasis;
                    ParseInlineCode;
                    ParseImageInline;
                ]

            textParagraphRules
            |> Seq.map (fun r -> r(input))
            |> Seq.filter Option.isSome
            |> Seq.fold (fun acc r -> if Option.isSome acc then acc else r) None

        let rec Parse input =
            match ParseInline input with
            | Some (ast, []) -> Some ([ast], [])
            | Some (ast, NewLineToken::NewLineToken::t) -> Some ([ast], t)
            | Some (ast, tokens) ->
                let nextOption = Parse tokens
                match nextOption with
                | Some (nexts, t) -> Some (ast::nexts, t)
                | None -> Some ([ast], tokens)
            | None -> None
        
        match Parse input with
        | Some (asts, t) -> Some(TextParagraphBlock(asts) :> IAbstractSyntaxTreeBlock, t)
        | _ -> None

    let rules : (Token list -> (IAbstractSyntaxTreeBlock * Token list) option) list = [
            ParseAtxTitle;
            ParseTextParagraph;
        ]

    type public MarkdownParser() =
        let rec parse inp =
            rules
            |> Seq.map (fun r -> r inp)
            |> Seq.filter Option.isSome
            |> Seq.map (fun r -> match r.Value with
                                 | (ast, []) -> Some ast
                                 | (ast, i) -> match parse i with
                                               | Some nextAst -> Some(ast.SetNext nextAst)
                                               | None -> None)
            |> Seq.fold (fun s res -> if Option.isSome s then s else res) None

        member public this.Parse input = (this :> IParser<_>).Parse input

        interface IParser<Token seq> with
            member __.Parse input = match parse [for t in input -> t] with
                                    | Some astb -> Some (astb :> IAbstractSyntaxTree)
                                    | None -> None
                