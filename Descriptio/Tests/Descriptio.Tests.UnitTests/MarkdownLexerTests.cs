using System.Collections.Generic;
using System.Linq;
using Descriptio.Factories;
using Descriptio.Tests.FluentAssertionsExtensions;
using FluentAssertions;
using Xunit;

using static Descriptio.Parser.MarkdownLexer;

// ReSharper disable InconsistentNaming

namespace Descriptio.Tests.UnitTests
{
    [Trait("Project", "Descriptio.Parser")]
    [Trait("Type", "Markdown.TextLexer")]
    public class MarkdownLexerTests
    {
        private readonly LexerFactory _lexerFactory = new LexerFactory();

        [Theory(DisplayName = "Lexer should lex title")]
        [InlineData("# Title", "Title", 1, 0)]
        [InlineData("## Level 2 ###", "Level 2", 2, 3)]
        public void Lexer_Title_ShouldReturnTitle(string source, string expectedTitle, int expectedLevel, int expectedClosingTokenCount)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            var expectedTitleLevelTokens = Enumerable.Range(0, expectedLevel).Select(_ => Token.TitleLevelToken);
            var expectedTitleToken = Token.NewTitleToken(expectedTitle);
            var expectedClosingTokens = Enumerable.Range(0, expectedClosingTokenCount).Select(_ => Token.TitleClosingToken);
            var expectedTokens = expectedTitleLevelTokens.Append(expectedTitleToken).Concat(expectedClosingTokens);

            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_LineBreak_ShouldReturnLines_Data = new[]
        {
            new object[]{ "Hello\r\nWorld!", new[] { Token.NewTextToken("Hello World!") } },
            new object[]{ "Hello\rWorld!", new[] { Token.NewTextToken("Hello World!") } },
            new object[]{ "Hello\nWorld!", new[] { Token.NewTextToken("Hello World!") } },
            new object[]{ "Hello\r\n\r\nWorld!", new[] { Token.NewTextToken("Hello"), Token.NewLineToken, Token.NewTextToken("World!") } },
            new object[]{ "Hello\r\rWorld!", new[] { Token.NewTextToken("Hello"), Token.NewLineToken, Token.NewTextToken("World!") } },
            new object[]{ "Hello\n\nWorld!", new[] { Token.NewTextToken("Hello"), Token.NewLineToken, Token.NewTextToken("World!") } },
            new object[]{ "*Hello\r\nWor*ld!", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello Wor"), Token.EmphasisEndToken, Token.NewTextToken("ld!") } },
        };

        [Theory(DisplayName = "Lexer should lex line breaks")]
        [MemberData(nameof(Lexer_LineBreak_ShouldReturnLines_Data))]
        public void Lexer_LineBreak_ShouldReturnLines(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_Emphasis_ShouldReturnEmphasisTokens_Data = new[]
        {
            new object[]{ "*Hello World!*", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "*Hello\r\nWorld!*", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "*Hello\nWorld!", new[] { Token.NewTextToken("*Hello World!") } },
            new object[]{ "*Hello World!**", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken, Token.NewTextToken("*")} },
            new object[]{ "_Hello World!_", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "_Hello\r\nWorld!_", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "_Hello\nWorld!", new[] { Token.NewTextToken("_Hello World!") } },
            new object[]{ "_Hello World!__", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken, Token.NewTextToken("_") } },
            new object[]{ "*Hello World!_", new[] { Token.NewTextToken("*Hello World!_") } },
            new object[]{ "_Hello World!*", new[] { Token.NewTextToken("_Hello World!*") } }
        };

        [Theory(DisplayName = "Lexer should lex emphasis inlines")]
        [MemberData(nameof(Lexer_Emphasis_ShouldReturnEmphasisTokens_Data))]
        public void Lexer_Emphasis_ShouldReturnEmphasisTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_Strong_ShouldReturnStrongTokens_Data = new[]
        {
            new object[]{ "**Hello World!**", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken } },
            new object[]{ "**Hello\r\nWorld!**", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken } },
            new object[]{ "**Hello\nWorld!", new[] { Token.NewTextToken("**Hello World!") } },
            new object[]{ "**Hello World!***", new[] {Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken, Token.NewTextToken("*") } },
            new object[]{ "__Hello World!__", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken } },
            new object[]{ "__Hello\r\nWorld!__", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken } },
            new object[]{ "__Hello\nWorld!", new[] { Token.NewTextToken("__Hello World!") } },
            new object[]{ "__Hello World!___", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken, Token.NewTextToken("_") } },
            new object[]{ "**Hello World!__", new[] { Token.NewTextToken("**Hello World!__") } },
            new object[]{ "__Hello World!**", new[] { Token.NewTextToken("__Hello World!**") } },
        };

        [Theory(DisplayName = "Lexer should lex strong inlines")]
        [MemberData(nameof(Lexer_Strong_ShouldReturnStrongTokens_Data))]
        public void Lexer_Strong_ShouldReturnStrongTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_InlineCode_ShouldReturnInlineCodeTokens_Data = new[]
        {
            new object[]{ "``Hello World!``", new[] { Token.InlineCodeStartToken, Token.NewTextToken("Hello World!"), Token.InlineCodeEndToken } },
            new object[]{ "``Hello\r\nWorld!``", new[] { Token.InlineCodeStartToken, Token.NewTextToken("Hello World!"), Token.InlineCodeEndToken } },
            new object[]{ "``Hello`World!``", new[] { Token.InlineCodeStartToken, Token.NewTextToken("Hello`World!"), Token.InlineCodeEndToken } },
            new object[]{ "`Hello World!`", new[] { Token.InlineCodeStartToken, Token.NewTextToken("Hello World!"), Token.InlineCodeEndToken } },
            new object[]{ "`Hello\r\nWorld!`", new[] { Token.InlineCodeStartToken, Token.NewTextToken("Hello World!"), Token.InlineCodeEndToken } },
        };

        [Theory(DisplayName = "Lexer should lex inline code")]
        [MemberData(nameof(Lexer_InlineCode_ShouldReturnInlineCodeTokens_Data))]
        public void Lexer_InlineCode_ShouldReturnInlineCodeTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_TextLine_ShouldLexIndentationCorrectly_Data = new[]
        {
            new object[]{ "  Hello World!", new[] { Token.NewTextToken("Hello World!") } },
            new object[]{ "  Hello\r\n  World!", new[] { Token.NewTextToken("Hello World!") } }
        };

        [Theory(DisplayName = "Lexer should lex indentation correctly")]
        [MemberData(nameof(Lexer_TextLine_ShouldLexIndentationCorrectly_Data))]
        public void Lexer_TextLine_ShouldLexIndentationCorrectly(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_Image_ShouldReturnImageTokens_Data = new[]
        {
            new object[]
            {
                "![Hello World](img.jpg)",
                new[]
                {
                    Token.ImageAltStartToken,
                    Token.NewTextToken("Hello World"),
                    Token.ImageAltEndToken,
                    Token.LinkStartToken,
                    Token.NewTextToken("img.jpg"),
                    Token.LinkEndToken
                }
            },
            new object[]
            {
                @"![Hello World](img.jpg ""Title"")",
                new[]
                {
                    Token.ImageAltStartToken,
                    Token.NewTextToken("Hello World"),
                    Token.ImageAltEndToken,
                    Token.LinkStartToken,
                    Token.NewTextToken("img.jpg"),
                    Token.NewTextToken("Title"),
                    Token.LinkEndToken

                }
            },
            new object[]
            {
                @"Here is an image: ![Hello World](img.jpg ""Title"")",
                new[]
                {
                    Token.NewTextToken("Here is an image: "),
                    Token.ImageAltStartToken,
                    Token.NewTextToken("Hello World"),
                    Token.ImageAltEndToken,
                    Token.LinkStartToken,
                    Token.NewTextToken("img.jpg"),
                    Token.NewTextToken("Title"),
                    Token.LinkEndToken
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex image")]
        [MemberData(nameof(Lexer_Image_ShouldReturnImageTokens_Data))]
        public void Lexer_Image_ShouldReturnImageTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_Hyperlink_ShouldReturnHyperlinkTokens_Data = new[]
        {
            new object[]
            {
                "[Hello World](http://example.com)",
                new[]
                {
                    Token.LinkTextStartToken,
                    Token.NewTextToken("Hello World"),
                    Token.LinkTextEndToken,
                    Token.LinkStartToken,
                    Token.NewTextToken("http://example.com"),
                    Token.LinkEndToken
                }
            },
            new object[]
            {
                @"[Hello World](http://example.com ""Example.com"")",
                new[]
                {
                    Token.LinkTextStartToken,
                    Token.NewTextToken("Hello World"),
                    Token.LinkTextEndToken,
                    Token.LinkStartToken,
                    Token.NewTextToken("http://example.com"),
                    Token.NewTextToken("Example.com"),
                    Token.LinkEndToken
                }
            },
            new object[]
            {
                @"This is a link to [Hello World](http://example.com ""Example.com"")",
                new[]
                {
                    Token.NewTextToken("This is a link to "),
                    Token.LinkTextStartToken,
                    Token.NewTextToken("Hello World"),
                    Token.LinkTextEndToken,
                    Token.LinkStartToken,
                    Token.NewTextToken("http://example.com"),
                    Token.NewTextToken("Example.com"),
                    Token.LinkEndToken
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex hyperlinks")]
        [MemberData(nameof(Lexer_Hyperlink_ShouldReturnHyperlinkTokens_Data))]
        public void Lexer_Hyperlink_ShouldReturnHyperlinkTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_Enumeration_ShouldReturnEnumerationTokens_Data = new[]
        {
            new object[]
            {
                "1. abc",
                new[]
                {
                    Token.NewEnumerationToken(0, 1),
                    Token.NewTextToken("abc"),
                }
            },
            new object[]
            {
                "1. *abc* **def** `ghi`",
                new[]
                {
                    Token.NewEnumerationToken(0, 1),
                    Token.EmphasisStartToken,
                    Token.NewTextToken("abc"),
                    Token.EmphasisEndToken,
                    Token.NewTextToken(" "),
                    Token.StrongStartToken,
                    Token.NewTextToken("def"),
                    Token.StrongEndToken,
                    Token.NewTextToken(" "),
                    Token.InlineCodeStartToken,
                    Token.NewTextToken("ghi"),
                    Token.InlineCodeEndToken
                }
            },
            new object[]
            {
                @"3. def
1. test
214781. test",
                new[]
                {
                    Token.NewEnumerationToken(0, 3),
                    Token.NewTextToken("def"),
                    Token.NewEnumerationToken(0, 1),
                    Token.NewTextToken("test"),
                    Token.NewEnumerationToken(0, 214781),
                    Token.NewTextToken("test"),
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex enumerations")]
        [MemberData(nameof(Lexer_Enumeration_ShouldReturnEnumerationTokens_Data))]
        public void Lexer_Enumeration_ShouldReturnEnumerationTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_UnorderedEnumeration_ShouldReturnEnumerationTokens_Data = new[]
        {
            new object[]
            {
                "* abc",
                new[]
                {
                    Token.NewUnorderedEnumerationToken(0, '*'),
                    Token.NewTextToken("abc"),
                }
            },
            new object[]
            {
                @"- def
    - test
        - test2",
                new[]
                {
                    Token.NewUnorderedEnumerationToken(0, '-'),
                    Token.NewTextToken("def"),
                    Token.NewUnorderedEnumerationToken(1, '-'),
                    Token.NewTextToken("test"),
                    Token.NewUnorderedEnumerationToken(2, '-'),
                    Token.NewTextToken("test2"),
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex unordered enumerations")]
        [MemberData(nameof(Lexer_UnorderedEnumeration_ShouldReturnEnumerationTokens_Data))]
        public void Lexer_UnorderedEnumeration_ShouldReturnEnumerationTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_CodeBlock_ShouldReturnCodeBlockTokens_Data = new[]
        {
            new object[]
            {
                @"```
Some code
```",
                new[]
                {
                    Token.CodeBlockStartToken,
                    Token.NewTextToken("Some code"),
                    Token.CodeBlockEndToken
                }
            },
            new object[]
            {
                @"```c#
int i = 0;
```",
                new[]
                {
                    Token.CodeBlockStartToken,
                    Token.NewCodeBlockLanguageToken("c#"),
                    Token.NewTextToken("int i = 0;"),
                    Token.CodeBlockEndToken
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex code blocks")]
        [MemberData(nameof(Lexer_CodeBlock_ShouldReturnCodeBlockTokens_Data))]
        public void Lexer_CodeBlock_ShouldReturnCodeBlockTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_Blockquote_ShouldReturnBlockquoteTokens_Data = new[]
        {
            new object[]
            {
                @"> This is a blockquote.",
                new[]
                {
                    Token.BlockquoteToken,
                    Token.NewTextToken("This is a blockquote.")
                }
            },
            new object[]
            {
                @"> *This is an emphasized blockquote*
> __This is a strong blockquote__
> ``This is a code inline blockquote``
> ![Hello World](img.jpg ""Title"")
> [Hello World](http://example.com ""Example.com"")",
                new[]
                {
                    Token.BlockquoteToken, Token.EmphasisStartToken, Token.NewTextToken("This is an emphasized blockquote"), Token.EmphasisEndToken,
                    Token.BlockquoteToken, Token.StrongStartToken, Token.NewTextToken("This is a strong blockquote"), Token.StrongEndToken,
                    Token.BlockquoteToken, Token.InlineCodeStartToken, Token.NewTextToken("This is a code inline blockquote"), Token.InlineCodeEndToken,
                    Token.BlockquoteToken,
                    Token.ImageAltStartToken,  Token.NewTextToken("Hello World"), Token.ImageAltEndToken,
                    Token.LinkStartToken, Token.NewTextToken("img.jpg"), Token.NewTextToken("Title"), Token.LinkEndToken,
                    Token.BlockquoteToken,
                    Token.LinkTextStartToken, Token.NewTextToken("Hello World"), Token.LinkTextEndToken,
                    Token.LinkStartToken, Token.NewTextToken("http://example.com"), Token.NewTextToken("Example.com"), Token.LinkEndToken,
                }
            },
            new object[]
            {
                @"> # This is a title inside the blockquote
> Some text

> A new blockquote",
                new[]
                {
                    Token.BlockquoteToken,
                    Token.TitleLevelToken,
                    Token.NewTitleToken("This is a title inside the blockquote"),
                    Token.BlockquoteToken,
                    Token.NewTextToken("Some text"),
                    Token.NewLineToken,
                    Token.BlockquoteToken,
                    Token.NewTextToken("A new blockquote")
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex blockquote")]
        [MemberData(nameof(Lexer_Blockquote_ShouldReturnBlockquoteTokens_Data))]
        public void Lexer_Blockquote_ShouldReturnBlockquoteTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value
                  .Should()
                  .Equal(expectedTokens);
        }
    }
}
