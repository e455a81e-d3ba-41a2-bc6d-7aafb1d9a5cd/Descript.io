using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Xunit;

namespace Descriptio.Tests.IntegrationTests.JsonFormatter
{
    public class JsonFormatterIntegrationTests
    {
        //[Fact]
        public void JsonFormatter_ShouldYieldDocument()
        {
            var htmlFormatter = new Transform.Json.JsonFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: new TextParagraphBlock(new[]
                    {
                        new CleanTextInline("This is a text."),
                    },
                    next: new TitleAst(
                        "Title 2",
                        level: 2,
                        next: new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
                        {
                            new CleanTextInline("This is another text. "),
                            new StrongTextInline("This should be strong."),
                            new CleanTextInline(" "),
                            new EmphasisTextInline("And this should be emphasized."),
                            new CleanTextInline(" "),
                            new CodeTextInline("This should be formatted as code."),
                        },
                            next: new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
                            {
                                new CleanTextInline("Here, we should have a new paragraph "),
                                new HyperlinkInline(text: "with a link", href: "http://example.com", title: "It is a title"),
                                new CleanTextInline("."),
                                new ImageInline(alt: "Alt", src: @"C:/Path/To/An/Image.jpg", title: "It has a title too"),
                            },
                                next: new EnumerationBlock(
                                    items: new[]
                                    {
                                        new EnumerationItem(indent: 0, number: 1, inlines: new[] { new CleanTextInline("This should be item 1.") }),
                                        new EnumerationItem(indent: 0, number: 2, inlines: new[] { new CleanTextInline("This should be the second item.")}),
                                        new EnumerationItem(indent: 0, number: 3, inlines: new[] { new CleanTextInline("Though, this should be item 3.")})
                                    }
                                )
                            )
                        )
                )));

            var expectedResult = @"{""Title"":{""Next"":{""Next"":{""Next"":{""Next"":{""Next"":{""Items"":[{""Inlines"":[{""Text"":""This should be item 1.""}],""Indent"":0,""Number"":1},{""Inlines"":[{""Text"":""This should be the second item.""}],""Indent"":0,""Number"":2},{""Inlines"":[{""Text"":""Though, this should be item 3.""}],""Indent"":0,""Number"":3}],""Next"":null},""Inlines"":[{""Text"":""Here, we should have a new paragraph ""},{""Href"":""http://example.com"",""Title"":""It is a title"",""Text"":""with a link""},{""Text"":"".""},{""Alt"":""Alt"",""Src"":""C:/Path/To/An/Image.jpg"",""Title"":""It has a title too""}]},""Inlines"":[{""Text"":""This is another text. ""},{""Text"":""This should be strong.""},{""Text"":"" ""},{""Text"":""And this should be emphasized.""},{""Text"":"" ""},{""Text"":""This should be formatted as code.""}]},""Text"":""Title 2"",""Level"":2},""Inlines"":[{""Text"":""This is a text.""}]},""Text"":""Title 1"",""Level"":1}}
{""Next"":{""Next"":{""Next"":{""Next"":{""Items"":[{""Inlines"":[{""Text"":""This should be item 1.""}],""Indent"":0,""Number"":1},{""Inlines"":[{""Text"":""This should be the second item.""}],""Indent"":0,""Number"":2},{""Inlines"":[{""Text"":""Though, this should be item 3.""}],""Indent"":0,""Number"":3}],""Next"":null}}]";        

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
