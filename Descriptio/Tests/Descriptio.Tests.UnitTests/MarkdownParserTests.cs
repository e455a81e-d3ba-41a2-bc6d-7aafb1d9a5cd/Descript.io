using System.Collections.Generic;
using Descriptio.Core.AST;
using Descriptio.Parser;
using Descriptio.Tests.FluentAssertionsExtensions;
using Microsoft.FSharp.Core;
using Xunit;

using static Descriptio.Parser.MarkdownLexer;

// ReSharper disable InconsistentNaming

namespace Descriptio.Tests.UnitTests
{
    [Trait("Project", "Descriptio.Parser")]
    [Trait("Type", "Markdown.TextParser")]
    public class MarkdownParserTests
    {
        public static readonly IEnumerable<object[]> Parser_AtxTitle_ShouldReturnTitle_Data = new[]
        {
            new object[]{ new[] { Token.TitleLevelToken, Token.NewTitleToken("Title 1") }, new TitleAst("Title 1", level: 1)},
            new object[]{ new[] { Token.TitleLevelToken, Token.TitleLevelToken, Token.TitleLevelToken, Token.NewTitleToken("### Hello World!") }, new TitleAst("### Hello World!", level: 3)},
            new object[]{ new[] { Token.TitleLevelToken, Token.NewTitleToken("Hello World!##"), Token.TitleClosingToken }, new TitleAst("Hello World!##", level: 1)},
        };

        [Theory(DisplayName = "Parser should parse ATX Title")]
        [MemberData(nameof(Parser_AtxTitle_ShouldReturnTitle_Data))]
        public void Parser_AtxTitle_ShouldReturnTitle(Token[] source, FSharpOption<IAbstractSyntaxTree> expected)
        {
            // Arrange
            var parser = new MarkdownParser.MarkdownParser();

            // Act
            var result = parser.Parse(source);

            // Assert
            result.Should().Be(expected);
        }

        public static readonly IEnumerable<object[]> Parser_TextLine_ShouldReturnTextLine_Data = new[]
        {
            new object[]
            {
                new[] { Token.NewTextToken("This is a text.") },
                new TextParagraphBlock(new[] { new CleanTextInline("This is a text.") })
            },
            new object[]
            {
                new[]
                {
                    Token.NewTextToken("This is an "),
                    Token.EmphasisStartToken,
                    Token.NewTextToken("emphasized"),
                    Token.EmphasisEndToken,
                    Token.NewTextToken(" text.")
                },
                new TextParagraphBlock(
                    new IAbstractSyntaxTreeInline[]
                    {
                        new CleanTextInline("This is an "),
                        new EmphasisTextInline("emphasized"),
                        new CleanTextInline(" text.")
                    })
            }
        };

        [Theory(DisplayName = "Parser should parse text line")]
        [MemberData(nameof(Parser_TextLine_ShouldReturnTextLine_Data))]
        public void Parser_TextLine_ShouldReturnTextLine(Token[] source, FSharpOption<IAbstractSyntaxTree> expected)
        {
            // Arrange
            var parser = new MarkdownParser.MarkdownParser();

            // Act
            var result = parser.Parse(source);

            // Assert
            result.Should().Be(expected);
        }
    }
}
