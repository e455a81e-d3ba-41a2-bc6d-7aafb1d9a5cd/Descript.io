using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Parser;
using Descriptio.Tests.UnitTests.Extensions;
using FluentAssertions;
using Xunit;

using static Descriptio.Parser.MarkdownLexer;

// ReSharper disable InconsistentNaming

namespace Descriptio.Tests.UnitTests
{
    [Trait("Project", "Descriptio.Parser")]
    [Trait("Type", "Markdown.TextParser")]
    public class MarkdownParserTests
    {
        ////public static readonly IEnumerable<object[]> Parser_AtxTitle_ShouldReturnTitle_Data = new[]
        ////{
        ////    new object[]{ new[] { Token.EmphasisStartToken, Token.NewTextToken("# Title 1"), Token.EmphasisEndToken }, new  TitleAst("Title 1", level: 1)},
        ////    new object[]{ new[] { Token.EmphasisStartToken, Token.NewTextToken("### Hello World!"), Token.EmphasisEndToken }, new TitleAst("Hello World", level: 3)}
        ////};

        ////[Theory]
        ////[MemberData(nameof(Parser_AtxTitle_ShouldReturnTitle_Data))]
        ////public void Parser_AtxTitle_ShouldReturnTitle(Token[] source, IAbstractSyntaxTree expected)
        ////{
        ////    // Arrange
        ////    var parser = new MarkdownParser.Parser();

        ////    // Act
        ////    var result = parser.Parse(source);

        ////    // Assert
        ////    result.Should()
        ////          .BeSome()
        ////          .And.Subject.Value.Should()
        ////          .Be(expected);
        ////}
    }
}
