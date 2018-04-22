using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Transform.Xml;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class MarkownXmlFormatterTests
    {
        [Fact]
        public void XmlFormatter_ShouldReturnTitle()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: null);

            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><title>Title 1</title></root>";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }

        }

        [Fact]
        public void XmlFormatter_ShouldReturnTextLine()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TextParagraphBlock(new[]
            {
                new CleanTextInline("This is a text."),
            });

            var expectedResult =
                @"<?xml version=""1.0"" encoding=""utf-8""?><root><text_paragraph><clean_text>This is a text.</clean_text></text_paragraph></root>";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void XmlFormatter_ShouldReturnEmphasis()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new EmphasisTextInline("This should be emphasized."),
            });

            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><text_paragraph><clean_text>This is a text. </clean_text><emphasised_text>This should be emphasized.</emphasised_text></text_paragraph></root>";
            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void XmlFormatter_ShouldReturnStrong()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text."),
                new StrongTextInline("This should be strong."),
            });

            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><text_paragraph><clean_text>This is a text.</clean_text><strong_text>This should be strong.</strong_text></text_paragraph></root>";
            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void XmlFormatter_ShouldReturnHyperlink()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("There is some text and a "),
                new HyperlinkInline(
                text: "link",
                href: "http://example.com",
                title: "It is a title"
            )});

            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><text_paragraph><clean_text>There is some text and a </clean_text><hyperlink><hyperlink_text>link</hyperlink_text><hyperlink_href>http://example.com</hyperlink_href></hyperlink></text_paragraph></root>";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void XmlFormatter_ShouldReturnImage()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TextParagraphBlock(new[]
            {
                new ImageInline(alt: "Alt", src: @"C:\Path\To\An\Image.jpg", title: "It has a title too"),
            });


            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><text_paragraph><image><image_src>C:\Path\To\An\Image.jpg</image_src><image_alt>Alt</image_alt><image_title>It has a title too</image_title></image></text_paragraph></root>";
            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void XmlFormatter_ShouldReturnCode()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new CodeTextInline("This should be some code."),
            });

            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><text_paragraph><clean_text>This is a text. </clean_text><code_text>This should be some code.</code_text></text_paragraph></root>";
            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void XmlFormatter_ShouldReturnEnumeration()
        {
            var latexFormatter = new XmlFormatter();

            var ast = new EnumerationBlock(
                items: new[]
                    {
                        new EnumerationItem(indent: 0, number: 1, inlines: new[] { new CleanTextInline("This should be item 1.") }),
                        new EnumerationItem(indent: 0, number: 2, inlines: new[] { new CleanTextInline("This should be the second item.")}),
                        new EnumerationItem(indent: 0, number: 1234, inlines: new[] { new CleanTextInline("Though, this should be item 3.")})
                    }
                );

            var expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><root><enumeration><item number=""1""><clean_text>This should be item 1.</clean_text></item><item number=""2""><clean_text>This should be the second item.</clean_text></item><item number=""1234""><clean_text>Though, this should be item 3.</clean_text></item></enumeration></root>";
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
