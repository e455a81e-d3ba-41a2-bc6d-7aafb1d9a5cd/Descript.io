using System;
using System.Collections.Generic;
using Descriptio.Core.AST;
using Descriptio.Extensions;
using Descriptio.Factories;
using Microsoft.FSharp.Core;
using static Descriptio.Parser.Core;
using static Descriptio.Parser.MarkdownLexer;

namespace Descriptio
{
    public static class Parse
    {
        private static readonly LexerFactory LexerFactory = new LexerFactory();
        private static readonly ParserFactory ParserFactory = new ParserFactory();

        public static ParseResult MarkdownString(string input)
            => MarkdownStringUsing(
                LexerFactory.CreateMarkdownTextLexerWithDefaultRules(),
                ParserFactory.CreateMarkdownParserWithDefaultRules(),
                input);

        public static ParseResult MarkdownStringUsing(ILexer<string, IEnumerable<Token>> lexer, IParser<IEnumerable<Token>> parser, string input)
        {
            if (lexer is null) throw new ArgumentNullException(nameof(lexer));
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            var lexerResult = lexer.Lex(input);
            if (lexerResult.IsSome())
            {
                return new ParseResult(FSharpOption<IAbstractSyntaxTreeBlock>.None);
            }

            var parserResult = parser.Parse(lexerResult.Value);
            return new ParseResult(parserResult);
        }
    }
}
