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
    /// <summary>
    /// Provides the fluent API for parsing.
    /// </summary>
    public static class Parse
    {
        private static readonly LexerFactory LexerFactory = new LexerFactory();
        private static readonly ParserFactory ParserFactory = new ParserFactory();

        /// <summary>
        /// Parses a markdown string using the default lexer and parser implementations.
        /// </summary>
        /// <param name="input">
        /// The markdown string to parse.
        /// </param>
        /// <returns>The result of the parsing process.</returns>
        public static ParseResult MarkdownString(string input)
            => MarkdownStringUsing(
                LexerFactory.CreateMarkdownTextLexerWithDefaultRules(),
                ParserFactory.CreateMarkdownParserWithDefaultRules(),
                input);

        /// <summary>
        /// Parses a markdown string using a specified lexer and parser.
        /// </summary>
        /// <param name="lexer">
        /// The lexer to use.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="parser">
        /// The parser to use.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="input">
        /// The markdown string to parse.
        /// </param>
        /// <returns>The result of the parsing process.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if
        /// <list type="bullet">
        /// <item><paramref name="lexer"/> is <c>null</c>.</item>
        /// <item><paramref name="parser"/> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        public static ParseResult MarkdownStringUsing(ILexer<string, IEnumerable<Token>> lexer, IParser<IEnumerable<Token>> parser, string input)
        {
            if (lexer is null) throw new ArgumentNullException(nameof(lexer));
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            var lexerResult = lexer.Lex(input);
            if (lexerResult.IsNone())
            {
                return new ParseResult(FSharpOption<IAbstractSyntaxTreeBlock>.None);
            }

            var parserResult = parser.Parse(lexerResult.Value);
            return new ParseResult(parserResult);
        }
    }
}
