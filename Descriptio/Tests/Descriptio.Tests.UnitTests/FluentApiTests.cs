using System;
using System.IO;
using Descriptio.Core.AST;
using Descriptio.Extensions;
using Descriptio.Factories;
using Descriptio.Tests.FluentAssertionsExtensions;
using FluentAssertions;
using Microsoft.FSharp.Core;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class FluentApiTests
    {
        private readonly LexerFactory _lexerFactory = new LexerFactory();
        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly FormatterFactory _formatterFactory = new FormatterFactory();

        [Fact(DisplayName = "Fluent API parsing markdown and transform to HTML should return the same as the factories.")]
        public void FluentApi_ParseMarkdownToHtml_ShouldReturnSameAsFactory()
        {
            // Arrange
            const string input = "# Hello World!";
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = _parserFactory.CreateMarkdownParserWithDefaultRules();
            var formatter = _formatterFactory.CreateHtmlFormatterWithDefaultRules();

            string expected;
            using (var stream = new MemoryStream())
            {
                var lexerResult = lexer.Lex(input);
                var parserResult = lexerResult.IsNone() ? FSharpOption<IAbstractSyntaxTreeBlock>.None : parser.Parse(lexerResult.Value);

                if (parserResult.IsNone())
                {
                    throw new InvalidOperationException();
                }

                formatter.Transform(parserResult.Value, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    expected = reader.ReadToEnd();
                }
            }

            // Act
            var actual = Parse.MarkdownString(input).AndFormatToHtmlString();

            // Assert
            actual.Should().BeSome().And.Subject.Value.Should().Be(expected);
        }

        [Fact(DisplayName = "Fluent API parsing markdown and transform to LaTex should return the same as the factories.")]
        public void FluentApi_ParseMarkdownToLaTex_ShouldReturnSameAsFactory()
        {
            // Arrange
            const string input = "# Hello World!";
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = _parserFactory.CreateMarkdownParserWithDefaultRules();
            var formatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            string expected;
            using (var stream = new MemoryStream())
            {
                var lexerResult = lexer.Lex(input);
                var parserResult = lexerResult.IsNone() ? FSharpOption<IAbstractSyntaxTreeBlock>.None : parser.Parse(lexerResult.Value);

                if (parserResult.IsNone())
                {
                    throw new InvalidOperationException();
                }

                formatter.Transform(parserResult.Value, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    expected = reader.ReadToEnd();
                }
            }

            // Act
            var actual = Parse.MarkdownString(input).AndFormatToLaTexString();

            // Assert
            actual.Should().BeSome().And.Subject.Value.Should().Be(expected);
        }

        [Fact(DisplayName = "Fluent API parsing markdown and transform to XML should return the same as the factories.")]
        public void FluentApi_ParseMarkdownToXml_ShouldReturnSameAsFactory()
        {
            // Arrange
            const string input = "# Hello World!";
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = _parserFactory.CreateMarkdownParserWithDefaultRules();
            var formatter = _formatterFactory.CreateXmlFormatterWithDefaultRules();

            string expected;
            using (var stream = new MemoryStream())
            {
                var lexerResult = lexer.Lex(input);
                var parserResult = lexerResult.IsNone() ? FSharpOption<IAbstractSyntaxTreeBlock>.None : parser.Parse(lexerResult.Value);

                if (parserResult.IsNone())
                {
                    throw new InvalidOperationException();
                }

                formatter.Transform(parserResult.Value, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    expected = reader.ReadToEnd();
                }
            }

            // Act
            var actual = Parse.MarkdownString(input).AndFormatToXmlString();

            // Assert
            actual.Should().BeSome().And.Subject.Value.Should().Be(expected);
        }

        [Fact(DisplayName = "Fluent API parsing markdown and transform to Json should return the same as the factories.")]
        public void FluentApi_ParseMarkdownToJson_ShouldReturnSameAsFactory()
        {
            // Arrange
            const string input = "# Hello World!";
            var lexer = _lexerFactory.CreateMarkdownTextLexerWithDefaultRules();
            var parser = _parserFactory.CreateMarkdownParserWithDefaultRules();
            var formatter = _formatterFactory.CreateJsonFormatterWithDefaultRules();

            string expected;
            using (var stream = new MemoryStream())
            {
                var lexerResult = lexer.Lex(input);
                var parserResult = lexerResult.IsNone() ? FSharpOption<IAbstractSyntaxTreeBlock>.None : parser.Parse(lexerResult.Value);

                if (parserResult.IsNone())
                {
                    throw new InvalidOperationException();
                }

                formatter.Transform(parserResult.Value, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    expected = reader.ReadToEnd();
                }
            }

            // Act
            var actual = Parse.MarkdownString(input).AndFormatToJsonString();

            // Assert
            actual.Should().BeSome().And.Subject.Value.Should().Be(expected);
        }
    }
}
