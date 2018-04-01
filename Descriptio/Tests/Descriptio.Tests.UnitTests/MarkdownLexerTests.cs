using System.Collections.Generic;
using System.Linq;
using Descriptio.Tests.UnitTests.Extensions;
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
        [Theory]
        [InlineData("****")]
        public void Lexer_ShouldLexSuccessfully(string source)
        {
            // Arrange
            var lexer = new TextLexer();

            // Act
            var result = lexer.Lex(source);

            // Assert
            result.Should().BeSome();
        }

        [Theory]
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
            new object[]{ "**Hello\r\nWor**ld!", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello Wor"), Token.EmphasisEndToken, Token.NewTextToken("ld!") } },
        };

        [Theory]
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

        public static readonly IEnumerable<object[]> Lexer_Emphasis_ShouldReturnBoldTokens_Data = new[]
        {
            new object[]{ "**Hello World!**", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "**Hello\r\nWorld!**", new[] { Token.EmphasisStartToken, Token.NewTextToken("Hello World!"), Token.EmphasisEndToken } },
            new object[]{ "**Hello\nWorld!", new[] { Token.NewTextToken("**Hello World!") } }
        };

        [Theory]
        [MemberData(nameof(Lexer_Emphasis_ShouldReturnBoldTokens_Data))]
        public void Lexer_Bold_ShouldReturnBoldTokens(string source, Token[] expectedTokens)
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
