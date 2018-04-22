using System.Collections.Generic;
using Descriptio.Parser;

using static Descriptio.Parser.Core;

namespace Descriptio.Factories
{
    public class LexerFactory
    {
        public ILexer<string, IEnumerable<MarkdownLexer.Token>> CreateMarkdownTextLexerWithDefaultRules()
            => new MarkdownLexer.TextLexer(MarkdownLexer.LexerDefaultRules);
    }
}
