using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Transform.Latex;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class MarkownLatexFormatterTests
    {
        [Fact]
        public void LatexFormatter_IteratesAllBlocks()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: new TextParagraphBlock(
                    new[]
                    {
                        new CleanTextInline("This is a text.")
                    }));

            var expectedResult = @"\section*{Title 1}
\paragraph{}
This is a text.
";
 
            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }

        }
    }
}
