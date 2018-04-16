using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Transform.Html;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class MarkownHtmlFormatterTests
    {
        [Fact]
        public void HtmlFormatter_ShouldReturnTitle()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: null);

            var expectedResult = @"<h1>Title 1</h1>";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }

        }

        [Fact]
        public void HtmlFormatter_ShouldReturnTextLine()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TextParagraphBlock(new []
            {
                new CleanTextInline("This is a text."),
            });

            var expectedResult = @"<p>
This is a text.
</p>";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void HtmlFormatter_ShouldReturnEnumeration()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new EnumerationBlock(
                items: new[]
                    {
                        new EnumerationItem(1, new[] { new CleanTextInline("This should be item 1.") }),
                        new EnumerationItem(2, new[] { new CleanTextInline("This should be the second item.")}),
                        new EnumerationItem(1234, new[] { new CleanTextInline("Though, this should be item 3.")})
                    }
                );

            var expectedResult = @"<ol>
<li>
This should be item 1.
</li>
<li>
This should be the second item.
</li>
<li>
Though, this should be item 3.
</li>
</ol>
";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
    }
}
