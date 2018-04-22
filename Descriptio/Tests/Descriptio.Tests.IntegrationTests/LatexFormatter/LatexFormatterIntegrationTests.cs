using System.Collections.Generic;
using System.IO;
using System.Text;
using Descriptio.Factories;
using Microsoft.FSharp.Core;
using Xunit;

using static Descriptio.Parser.MarkdownLexer;

namespace Descriptio.Tests.IntegrationTests.LatexFormatter
{
    public class LatexFormatterIntegrationTests
    {
        private readonly LexerFactory _lexerFactory = new LexerFactory();
        private readonly ParserFactory _parserFactory = new ParserFactory();

        [Fact]
        public void LatexFormatter_ShouldYieldDocument()
        {
            var markdown = "# Title 1\r\nThis is a text.\r\n**This should be strong.**\r\n*And this should be emphasized.*\r\n`This should be formatted as code.`\r\n\r\nHere, we should have a new paragraph\r\n[with a link](http://example.com \"It is a title\")\r\nand an image ![Alt](C:\\Path\\To\\An\\Image.jpg, \"It has a title too\")\r\n\r\n1. This should be item 1.\r\n2. This should be the second item.\r\n1234. Though, this should be item 3.";

            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = _parserFactory.CreateMarkdownParserWithDefaultRules();

            // Act & Assert
            var lexerResult = lexer.Lex(markdown);

            Assert.True(FSharpOption<IEnumerable<Token>>.get_IsSome(lexerResult));

            var output = lexerResult.Value;
            var parserResult = parser.Parse(output);

            var latexFormatter = new Transform.Latex.LatexFormatter();
            var expectedResult = "\\section*{Title 1}\r\n\\begin{paragraph}{}\r\nThis is a text. \\textbf{This should be strong.} \\emph{And this should be emphasized.} \\begin{verbatim}\r\nThis should be formatted as code.\r\n\\end{verbatim}\r\n\r\n\\end{paragraph}\r\n\\begin{paragraph}{}\r\nHere, we should have a new paragraph \\href{http://example.com}{with a link} and an image \\begin{figure}[h]\r\n\\centering\r\n\\includegraphics{C:\\Path\\To\\An\\Image.jpg,}\r\n\\caption{It has a title too}\r\n\\end{figure}\r\n\r\n\\end{paragraph}\r\n\\begin{itemize}\r\n\\item [1] This should be item 1.\r\n\\item [2] This should be the second item.\r\n\\item [1234] Though, this should be item 3.\r\n\\end{itemize}\r\n";
            
            var memoryStream = new MemoryStream();
            latexFormatter.Transform(parserResult.Value, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
    }
}
