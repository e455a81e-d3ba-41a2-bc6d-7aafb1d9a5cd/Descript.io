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

            var expectedResult = @"<h1>Title 1</h1>
";

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

            var ast = new TextParagraphBlock(new[]
            {
                new CleanTextInline("This is a text."),
            });

            var expectedResult = @"<p>
This is a text.</p>
";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void HtmlFormatter_ShouldReturnEmphasis()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new EmphasisTextInline("This should be emphasized."),
            });

            var expectedResult = @"<p>
This is a text. <em>This should be emphasized.</em></p>
";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void HtmlFormatter_ShouldReturnStrong()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new StrongTextInline("This should be strong."),
            });

            var expectedResult = @"<p>
This is a text. <strong>This should be strong.</strong></p>
";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void HtmlFormatter_ShouldReturnHyperlink()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("There is some text and a "),
                new HyperlinkInline(
                text: "link", 
                href: "http://example.com", 
                title: "It is a title"
            )});

            var expectedResult = @"<p>
There is some text and a <a href=""http://example.com"">link</a></p>
";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void HtmlFormatter_ShouldReturnImage()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TextParagraphBlock(new []
            {
                new ImageInline(alt: "Alt", src: @"C:\Path\To\An\Image.jpg", title: "It has a title too"),
            });

            var expectedResult = @"<p>
<figure>
<img src=""C:\Path\To\An\Image.jpg"" alt=""Alt""/>
<figcaption>It has a title too</figcaption>
</figure>
</p>
";

            var memoryStream = new MemoryStream();
            htmlFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void HtmlFormatter_ShouldReturnCode()
        {
            var htmlFormatter = new HtmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new CodeTextInline("This should be some code."),
            });

            var expectedResult = @"<p>
This is a text. <code>This should be some code.</code></p>
";

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
                        new EnumerationItem(indent: 0, number: 1, inlines: new[] { new CleanTextInline("This should be item 1.") }),
                        new EnumerationItem(indent: 0, number: 2, inlines: new[] { new CleanTextInline("This should be the second item.")}),
                        new EnumerationItem(indent: 0, number: 1234, inlines: new[] { new CleanTextInline("Though, this should be item 3.")})
                    }
                );

            var expectedResult = @"<ol>
<li>This should be item 1.</li>
<li>This should be the second item.</li>
<li>Though, this should be item 3.</li>
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
