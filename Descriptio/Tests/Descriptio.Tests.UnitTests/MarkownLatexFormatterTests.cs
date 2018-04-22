using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Factories;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class MarkownLatexFormatterTests
    {
        private readonly FormatterFactory _formatterFactory = new FormatterFactory();

        [Fact]
        public void LatexFormatter_ShouldReturnTitle()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TitleAst(
                "Title 1",
                level: 1);

            string expectedResult = @"\section*{Title 1}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void LatexFormatter_ShouldReturnTextLine()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TextParagraphBlock(new[]
            {
                new CleanTextInline("This is a text."),
            });

            string expectedResult = @"\begin{paragraph}{}
This is a text.
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void LatexFormatter_ShouldReturnEmphasis()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new EmphasisTextInline("This should be emphasized."),
            });

            string expectedResult = @"\begin{paragraph}{}
This is a text. \emph{This should be emphasized.}
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnStrong()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text."),
                new StrongTextInline("This should be strong."),
            });

            string expectedResult = @"\begin{paragraph}{}
This is a text.\textbf{This should be strong.}
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnHyperlink()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("There is some text and a "),
                new HyperlinkInline(
                text: "link", 
                href: "http://example.com", 
                title: "It is a title"
            )});

            string expectedResult = @"\begin{paragraph}{}
There is some text and a \href{http://example.com}{link}
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnImage()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TextParagraphBlock(new []
            {
                new ImageInline(alt: "Alt", src: @"C:\Path\To\An\Image.jpg", title: "It has a title too"),
            });


            string expectedResult = @"\begin{paragraph}{}
\begin{figure}[h]
\centering
\includegraphics{C:\Path\To\An\Image.jpg}
\caption{It has a title too}
\end{figure}

\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnCode()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new CodeTextInline("This should be some code."),
            });

            string expectedResult = @"\begin{paragraph}{}
This is a text. \begin{verbatim}
This should be some code.
\end{verbatim}

\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void LatexFormatter_ShouldReturnEnumeration()
        {
            var latexFormatter = _formatterFactory.CreateLaTexFormatterWithDefaultRules();

            var ast = new EnumerationBlock(
                items: new[]
                    {
                        new EnumerationItem(indent: 0, number: 1, inlines: new[] { new CleanTextInline("This should be item 1.") }),
                        new EnumerationItem(indent: 0, number: 2, inlines: new[] { new CleanTextInline("This should be the second item.")}),
                        new EnumerationItem(indent: 0, number: 1234, inlines: new[] { new CleanTextInline("Though, this should be item 3.")})
                    }
                );

            string expectedResult = @"\begin{itemize}
\item [1] This should be item 1.
\item [2] This should be the second item.
\item [1234] Though, this should be item 3.
\end{itemize}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
    }
}
