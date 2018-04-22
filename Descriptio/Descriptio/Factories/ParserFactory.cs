using System.Collections.Generic;
using Descriptio.Parser;

namespace Descriptio.Factories
{
    public class ParserFactory
    {
        public Parser.Core.IParser<IEnumerable<MarkdownLexer.Token>> CreateMarkdownParserWithDefaultRules() => new MarkdownParser.Parser(MarkdownParser.ParserDefaultRules);
    }
}
