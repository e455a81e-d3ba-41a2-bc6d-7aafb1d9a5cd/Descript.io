namespace Descriptio.Parser

open Descriptio.Core.AST
open Descriptio.Parser.Core
open Descriptio.Parser.MarkdownLexer

module public MarkdownParser =
    type ParserRule = Token list -> (IAbstractSyntaxTreeBlock * Token list) option

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

    let ParseInline input =
        let ParseCleanText inp =
            match inp with
            | TextToken(txt)::t -> Some(CleanTextInline(txt) :> IAbstractSyntaxTreeInline, t)
            | _ -> None
        
        let ParseEmphasis inp =
            let ParseEmphasisStart i =
                match i with
                | EmphasisStartToken::t -> Some t
                | _ -> None

            let ParseEmphasisText i =
                match i with
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
            
        let ParseHyperlinkInline inp = 
            let ParseHyperlinkText i =
                match i with
                | LinkTextStartToken::TextToken(text)::LinkTextEndToken::t -> Some(t, text)
                | _ -> None
                
            let ParseHyperlinkHrefTitle i =
                match i with
                | Some(LinkStartToken::TextToken(href)::TextToken(title)::LinkEndToken::t, text) ->
                    Some(HyperlinkInline(text, href, title) :> IAbstractSyntaxTreeInline, t)
                | Some(LinkStartToken::TextToken(href)::LinkEndToken::t, text) ->
                    Some(HyperlinkInline(text, href) :> IAbstractSyntaxTreeInline, t)
                | _ -> None
                
            inp |> ParseHyperlinkText |> ParseHyperlinkHrefTitle

        let textParagraphRules = [
                ParseCleanText;
                ParseStrong;
                ParseEmphasis;
                ParseInlineCode;
                ParseImageInline;
                ParseHyperlinkInline;
            ]

        textParagraphRules
        |> Seq.map (fun r -> r(input))
        |> Seq.filter Option.isSome
        |> Seq.fold (fun acc r -> if Option.isSome acc then acc else r) None

    let rec ParseInlineRec lineDelimiter keepDelimiter input =
        match ParseInline input with
        | Some (ast, []) -> Some ([ast], [])
        | Some (ast, d::t) when lineDelimiter d -> Some ([ast], if keepDelimiter then d::t else t)
        | Some (ast, tokens) ->
            let nextOption = ParseInlineRec lineDelimiter keepDelimiter tokens
            match nextOption with
            | Some (nexts, t) -> Some (ast::nexts, t)
            | None -> Some ([ast], tokens)
        | None -> None
        
    
    let ParseTextParagraph input =
        match ParseInlineRec ((=) NewLineToken) false input with
        | Some (asts, t) -> Some(TextParagraphBlock(asts) :> IAbstractSyntaxTreeBlock, t)
        | _ -> None
    
    let ParseCodeBlock input =
        let ParseCodeBlockBegin input =
            match input with
            | CodeBlockStartToken::t -> Some([], t)
            | _ -> None

        let ParseCodeBlockLanguage inp =
            match inp with
            | None -> None
            | Some(lines, input) -> match input with
                                    | CodeBlockLanguageToken(lang)::t -> Some(lang, lines, t)
                                    | _ -> Some("", lines, input)

        let rec ParseCodeBlockContent inp =
            match inp with
            | None -> None
            | Some (lang, lines, input) ->
                match input with
                | TextToken(txt)::t -> ParseCodeBlockContent(Some(lang, lines@[txt], t))
                | CodeBlockEndToken::t -> Some(lang, lines, t)
                | _ -> None

        match input |> ParseCodeBlockBegin |> ParseCodeBlockLanguage |> ParseCodeBlockContent with
        | Some(lang, lines, t) -> Some(CodeBlock(lang, lines) :> IAbstractSyntaxTreeBlock, t)
        | _ -> None
        
        
    
    let ParseUnorderedEnumeration input =
        let rec ParseUnorderedEnumerationItems input =
            let ParseUnorderedEnumerationToken input =
                match input with
                | UnorderedEnumerationToken(indent, bullet)::t -> Some(indent, bullet, t)
                | _ -> None
            
            match ParseUnorderedEnumerationToken input with
            | None -> None
            | Some(indent, bullet, inp) ->
                let inlinesOption = ParseInlineRec (fun t -> match t with UnorderedEnumerationToken(_, _) | NewLineToken -> true | _ -> false) true inp

                match inlinesOption with
                | Some (asts, []) -> Some([UnorderedEnumerationItem(indent, bullet, asts)], [])
                | Some (asts, NewLineToken::t) -> Some([UnorderedEnumerationItem(indent, bullet, asts)], t)
                | Some (asts, UnorderedEnumerationToken(i, b)::t) ->
                    let nextOption = ParseUnorderedEnumerationItems (UnorderedEnumerationToken(i, b)::t)
                    match nextOption with
                    | Some (next, t) -> Some(UnorderedEnumerationItem(indent, bullet, asts)::next, t)
                    | _ -> None
                | _ -> None

        match ParseUnorderedEnumerationItems input with
        | Some (asts, t) -> Some(UnorderedEnumerationBlock(asts) :> IAbstractSyntaxTreeBlock, t)
        | _ -> None

    
    let ParseEnumeration input =
        let rec ParseEnumerationItems input =
            let ParseEnumerationToken input =
                match input with
                | EnumerationToken(num)::t -> Some(num, t)
                | _ -> None
            
            match ParseEnumerationToken input with
            | None -> None
            | Some(num, inp) ->
                let inlinesOption = ParseInlineRec (fun t -> match t with EnumerationToken(_) | NewLineToken -> true | _ -> false) true inp

                match inlinesOption with
                | Some (asts, []) -> Some([EnumerationItem(num, asts)], [])
                | Some (asts, NewLineToken::t) -> Some([EnumerationItem(num, asts)], t)
                | Some (asts, EnumerationToken(n)::t) ->
                    let nextOption = ParseEnumerationItems (EnumerationToken(n)::t)
                    match nextOption with
                    | Some (next, t) -> Some(EnumerationItem(num, asts)::next, t)
                    | _ -> None
                | _ -> None

        match ParseEnumerationItems input with
        | Some (asts, t) -> Some(EnumerationBlock(asts) :> IAbstractSyntaxTreeBlock, t)
        | _ -> None

    let rules : (Token list -> (IAbstractSyntaxTreeBlock * Token list) option) list = [
            ParseAtxTitle;
            ParseCodeBlock;
            ParseEnumeration;
            ParseUnorderedEnumeration;
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
            |> Seq.tryPick (fun s -> s)

        member public this.Parse input = (this :> IParser<_>).Parse input

        interface IParser<Token seq> with
            member __.Parse input = match parse [for t in input -> t] with
                                    | Some astb -> Some (astb :> IAbstractSyntaxTree)
                                    | None -> None
                