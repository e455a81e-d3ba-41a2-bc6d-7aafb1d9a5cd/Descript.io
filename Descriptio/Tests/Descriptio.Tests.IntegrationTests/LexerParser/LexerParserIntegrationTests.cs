using System.Collections.Generic;
using Descriptio.Core.AST;
using Descriptio.Factories;
using Descriptio.Tests.FluentAssertionsExtensions;
using FluentAssertions;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Descriptio.Tests.IntegrationTests.LexerParser
{
    [Trait("Project", "Descriptio.Parser")]
    [Trait("Type", "Markdown.TextParser")]
    [Trait("Type", "Markdown.TextLexer")]
    public class LexerParserIntegrationTests
    {
        private readonly LexerFactory _lexerFactory = new LexerFactory();
        private readonly ParserFactory _parserFactory = new ParserFactory();

        public static readonly IEnumerable<object[]> LexerParser_string_ShouldYieldDocument_Data = new[]
        {
            new object[]
            {
                @"# Title 1
This is a text.",
                new TitleAst(
                    "Title 1",
                    level: 1,
                    next: new TextParagraphBlock(
                        new[]
                        {
                            new CleanTextInline("This is a text.")
                        }))
            },
            new object[]
            {
                @"# Title 1
This is a text.
**This should be strong.**
*And this should be emphasized.*
`This should be formatted as code.`

Here, we should have a new paragraph
[with a link](http://example.com ""It is a title"")
and an image ![Alt](C:\Path\To\An\Image.jpg, ""It has a title too"")

1. This should be item 1.
2. This should be the second item.
1234. Though, this should be item 3.",
                new TitleAst(
                    "Title 1",
                    level: 1,
                    next: new TextParagraphBlock(
                        new IAbstractSyntaxTreeInline[]
                        {
                            new CleanTextInline("This is a text. "),
                            new StrongTextInline("This should be strong."),
                            new CleanTextInline(" "),
                            new EmphasisTextInline("And this should be emphasized."),
                            new CodeTextInline("This should be formatted as code.")
                        },
                        next: new TextParagraphBlock(
                            new IAbstractSyntaxTreeInline[]
                            {
                                new CleanTextInline("Here, we should have a new paragraph "),
                                new HyperlinkInline(text: "with a link", href: "http://example.com", title: "It is a title"),
                                new ImageInline(alt: "Alt", src: @"C:\Path\To\An\Image.jpg", title: "It has a title too"),
                            },
                            next: new EnumerationBlock(
                                items: new[]
                                {
                                    new EnumerationItem(indent: 0, number: 1, inlines: new[] { new CleanTextInline("This should be item 1.") }),
                                    new EnumerationItem(indent: 0, number: 2, inlines: new[] { new CleanTextInline("This should be the second item.")}),
                                    new EnumerationItem(indent: 0, number: 1234, inlines: new[] { new CleanTextInline("Though, this should be item 3.")})
                                }))))
            },
        };

        [Theory(DisplayName = "Parsing process should yield simple document")]
        [MemberData(nameof(LexerParser_string_ShouldYieldDocument_Data))]
        public void LexerParser_string_ShouldYieldDocument(string input, IAbstractSyntaxTree expected)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = _parserFactory.CreateMarkdownParserWithDefaultRules();

            // Act & Assert
            var lexerResult = lexer.Lex(input);
            lexerResult.Should().BeSome();

            var output = lexerResult.Value;
            var parserResult = parser.Parse(output);
            parserResult.Should().BeSome().And.Subject.Value.Should().Be(expected);
        }
    }
}
