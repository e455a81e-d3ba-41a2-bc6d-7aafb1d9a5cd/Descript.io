using Descriptio.Core.AST;
using Descriptio.Extensions;
using Descriptio.Factories;
using Microsoft.FSharp.Core;

namespace Descriptio
{
    public static class Parse
    {
        private static readonly LexerFactory LexerFactory = new LexerFactory();
        private static readonly ParserFactory ParserFactory = new ParserFactory();

        public static ParseResult MarkdownString(string input)
        {
            var lexer = LexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = ParserFactory.CreateMarkdownParserWithDefaultRules();

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
