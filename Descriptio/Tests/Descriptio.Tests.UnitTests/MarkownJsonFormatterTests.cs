using System;
using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Transform.Json;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class MarkownJsonFormatterTests
    {
        [Fact]
        public void JsonFormatter_ShouldReturnTitle()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: null);

            var expectedResult = @"{""Title"":{""Next"":null,""Text"":""Title 1"",""Level"":1}}
";
        
            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }

        }

        [Fact]
        public void JsonFormatter_ShouldReturnTextLine()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TextParagraphBlock(new[]
            {
                new CleanTextInline("This is a text."),
            });

            var expectedResult = @"{""Next"":null,""Inlines"":[{""Text"":""This is a text.""}]}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void JsonFormatter_ShouldReturnEmphasis()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new EmphasisTextInline("This should be emphasized."),
            });

            var expectedResult = @"{""Next"":null,""Inlines"":[{""Text"":""This is a text. ""},{""Text"":""This should be emphasized.""}]}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void JsonFormatter_ShouldReturnStrong()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text."),
                new StrongTextInline("This should be strong."),
            });
            
            var expectedResult = @"{""Next"":null,""Inlines"":[{""Text"":""This is a text.""},{""Text"":""This should be strong.""}]}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void JsonFormatter_ShouldReturnHyperlink()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("There is some text and a "),
                new HyperlinkInline(
                text: "link", 
                href: "http://example.com", 
                title: "It is a title"
            )});

            var expectedResult = @"{""Next"":null,""Inlines"":[{""Text"":""There is some text and a ""},{""Href"":""http://example.com"",""Title"":""It is a title"",""Text"":""link""}]}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void JsonFormatter_ShouldReturnImage()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TextParagraphBlock(new []
            {
                new ImageInline(alt: "Alt", src: @"C:/Path/To/An/Image.jpg", title: "It has a title too"),
            });


            var expectedResult = @"{""Next"":null,""Inlines"":[{""Alt"":""Alt"",""Src"":""C:/Path/To/An/Image.jpg"",""Title"":""It has a title too""}]}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void JsonFormatter_ShouldReturnCode()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new CodeTextInline("This should be some code."),
            });

            var expectedResult = @"{""Next"":null,""Inlines"":[{""Text"":""This is a text. ""},{""Text"":""This should be some code.""}]}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void JsonFormatter_ShouldReturnEnumeration()
        {
            var JsonFormatter = new JsonFormatter();

            var ast = new EnumerationBlock(
                items: new[]
                    {
                        new EnumerationItem(indent: 0, number: 1, inlines: new[] { new CleanTextInline("This should be item 1.") }),
                        new EnumerationItem(indent: 0, number: 2, inlines: new[] { new CleanTextInline("This should be the second item.")}),
                        new EnumerationItem(indent: 0, number: 1234, inlines: new[] { new CleanTextInline("Though, this should be item 3.")})
                    }
                );

            var expectedResult = @"{""Items"":[{""Inlines"":[{""Text"":""This should be item 1.""}],""Indent"":0,""Number"":1},{""Inlines"":[{""Text"":""This should be the second item.""}],""Indent"":0,""Number"":2},{""Inlines"":[{""Text"":""Though, this should be item 3.""}],""Indent"":0,""Number"":1234}],""Next"":null}
";

            var memoryStream = new MemoryStream();
            JsonFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }


    }
}
