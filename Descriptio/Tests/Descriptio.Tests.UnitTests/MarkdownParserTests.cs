using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Descriptio.Parser;
using FluentAssertions;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    [Trait("Project", "Descriptio.Parser")]
    [Trait("Type", "Markdown.StreamParser")]
    public class MarkdownParserTests
    {
        public void Parser_ShouldParseSuccessful(string source)
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData("**a**")]
        public void Lexer_ShouldLexSuccessfully(string source)
        {
            // ARRANGE
            var lexer = new Markdown.TextLexer();

            // ACT
            var result = lexer.Lex(source);

            // ASSERT
            result.Should().NotBeNull();
        }
    }
}
