using System.Collections.Generic;
using System.Linq;
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
        [Theory(DisplayName = "Lexer should lex title")]
        [InlineData("# Title", "Title", 1, 0)]
        [InlineData("## Level 2 ###", "Level 2", 2, 3)]
        public void Lexer_Title_ShouldReturnTitle(string source, string expectedTitle, int expectedLevel, int expectedClosingTokenCount)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            var expectedTitleLevelTokens = Enumerable.Range(0, expectedLevel).Select(_ => Token.TitleLevelToken);
            var expectedTitleToken = Token.NewTitleToken(expectedTitle);
            var expectedClosingTokens = Enumerable.Range(0, expectedClosingTokenCount).Select(_ => Token.TitleClosingToken);
            var expectedTokens = expectedTitleLevelTokens.Append(expectedTitleToken).Concat(expectedClosingTokens);

            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
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
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_Emphasis_ShouldReturnEmphasisTokens_Data = new[]
        {
            new object[]{ "*Hello World!*", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "*Hello\r\nWorld!*", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "*Hello\nWorld!", new[] { Token.NewTextToken("*Hello World!") } },
            new object[]{ "*Hello World!**", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken, Token.NewTextToken("*")} }
        };

        [Theory(DisplayName = "Lexer should lex emphasis inlines")]
        [MemberData(nameof(Lexer_Emphasis_ShouldReturnEmphasisTokens_Data))]
        public void Lexer_Emphasis_ShouldReturnEmphasisTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
                  .Equal(expectedTokens);
        }

        public static readonly IEnumerable<object[]> Lexer_Strong_ShouldReturnStrongTokens_Data = new[]
        {
            new object[]{ "**Hello World!**", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken } },
            new object[]{ "**Hello\r\nWorld!**", new[] { Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken } },
            new object[]{ "**Hello\nWorld!", new[] { Token.EmphasisStartToken, Token.EmphasisEndToken, Token.NewTextToken("Hello World!") } },
            new object[]{ "**Hello World!***", new[] {Token.StrongStartToken, Token.NewTextToken("Hello World!"), Token.StrongEndToken, Token.NewTextToken("*")}},
        };

        [Theory(DisplayName = "Lexer should lex strong inlines")]
        [MemberData(nameof(Lexer_Strong_ShouldReturnStrongTokens_Data))]
        public void Lexer_Strong_ShouldReturnStrongTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
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
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
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
        };

        [Theory(DisplayName = "Lexer should lex image")]
        [MemberData(nameof(Lexer_Image_ShouldReturnImageTokens_Data))]
        public void Lexer_Image_ShouldReturnImageTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
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
        };

        [Theory(DisplayName = "Lexer should lex hyperlinks")]
        [MemberData(nameof(Lexer_Hyperlink_ShouldReturnHyperlinkTokens_Data))]
        public void Lexer_Hyperlink_ShouldReturnHyperlinkTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
                  .Equal(expectedTokens);
        }
        
        public static readonly IEnumerable<object[]> Lexer_Enumeration_ShouldReturnEnumerationTokens_Data = new[]
        {
            new object[]
            {
                "1. abc",
                new[]
                {
                    Token.NewEnumerationToken(1),
                    Token.NewTextToken("abc"),
                }
            },
            new object[]
            {
                @"3. def
1. test
214781. test",
                new[]
                {
                    Token.NewEnumerationToken(3),
                    Token.NewTextToken("def"),
                    Token.NewEnumerationToken(1),
                    Token.NewTextToken("test"),
                    Token.NewEnumerationToken(214781),
                    Token.NewTextToken("test"),
                }
            },
        };

        [Theory(DisplayName = "Lexer should lex enumerations")]
        [MemberData(nameof(Lexer_Enumeration_ShouldReturnEnumerationTokens_Data))]
        public void Lexer_Enumeration_ShouldReturnEnumerationTokens(string source, Token[] expectedTokens)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should()
                  .BeSome()
                  .And.Subject.Value.Item4
                  .Should<Token>()
                  .Equal(expectedTokens);
        }
    }
}
