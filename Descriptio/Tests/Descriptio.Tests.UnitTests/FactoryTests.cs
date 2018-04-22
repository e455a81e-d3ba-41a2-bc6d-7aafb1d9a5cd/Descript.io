using Descriptio.Factories;
using Descriptio.Parser;
using Descriptio.Transform.Html;
using Descriptio.Transform.Json;
using Descriptio.Transform.Latex;
using Descriptio.Transform.Xml;
using FluentAssertions;
using Xunit;

using static Descriptio.Parser.MarkdownLexer;

namespace Descriptio.Tests.UnitTests
{
    public class FactoryTests
    {
        [Fact(DisplayName = "LexerFactory create text lexer with default rules should return a text lexer")]
        public void LexerFactory_CreateDefaultTextLexer_ShouldReturnTextLexer()
        {
            // Arrange
            var factory = new LexerFactory();

            // Act
            var lexer = factory.CreateMarkdownTextLexerWithDefaultRules();

            // Assert
            lexer.Should().BeOfType<TextLexer>();
        }

        [Fact(DisplayName = "ParserFactory create parser with default rules should return a parser")]
        public void ParserFactory_CreateDefaultParser_ShouldReturnParser()
        {
            // Arrange
            var factory = new ParserFactory();

            // Act
            var parser = factory.CreateMarkdownParserWithDefaultRules();

            // Assert
            parser.Should().BeOfType<MarkdownParser.Parser>();
        }

        [Fact(DisplayName = "FormatterFactory create HTML formatter with default rules should return an HTML formatter")]
        public void FormatterFactory_CreateHtmlFormatter_ShouldReturnHtmlFormatter()
        {
            // Arrange
            var factory = new FormatterFactory();

            // Act
            var formatter = factory.CreateHtmlFormatterWithDefaultRules();

            // Assert
            formatter.Should().BeOfType<HtmlFormatter>();
        }

        [Fact(DisplayName = "FormatterFactory create LaTex formatter with default rules should return a LaTex formatter")]
        public void FormatterFactory_CreateLaTexFormatter_ShouldReturnLaTexFormatter()
        {
            // Arrange
            var factory = new FormatterFactory();

            // Act
            var formatter = factory.CreateLaTexFormatterWithDefaultRules();

            // Assert
            formatter.Should().BeOfType<LatexFormatter>();
        }

        [Fact(DisplayName = "FormatterFactory create XML formatter with default rules should return an XML formatter")]
        public void FormatterFactory_CreateXmlFormatter_ShouldReturnXmlFormatter()
        {
            // Arrange
            var factory = new FormatterFactory();

            // Act
            var formatter = factory.CreateXmlFormatterWithDefaultRules();

            // Assert
            formatter.Should().BeOfType<XmlFormatter>();
        }

        [Fact(DisplayName = "FormatterFactory create JSON formatter with default rules should return an JSON formatter")]
        public void FormatterFactory_CreateJsonFormatter_ShouldReturnJsonFormatter()
        {
            // Arrange
            var factory = new FormatterFactory();

            // Act
            var formatter = factory.CreateJsonFormatterWithDefaultRules();

            // Assert
            formatter.Should().BeOfType<JsonFormatter>();
        }
    }
}
