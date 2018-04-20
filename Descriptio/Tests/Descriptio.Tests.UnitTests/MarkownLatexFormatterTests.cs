using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Descriptio.Core.AST;
using Descriptio.Transform.Latex;
using Xunit;

namespace Descriptio.Tests.UnitTests
{
    public class MarkownLatexFormatterTests
    {
        [Fact]
        public void LatexFormatter_ShouldReturnTitle()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: null);

            var expectedResult = @"\section*{Title 1}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }

        }

        [Fact]
        public void LatexFormatter_ShouldReturnTextLine()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TextParagraphBlock(new[]
            {
                new CleanTextInline("This is a text."),
            });

            var expectedResult = @"\begin{paragraph}{}
This is a text.
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void LatexFormatter_ShouldReturnEmphasis()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new EmphasisTextInline("This should be emphasized."),
            });

            var expectedResult = @"\begin{paragraph}{}
This is a text. \emph{This should be emphasized.}
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnStrong()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text."),
                new StrongTextInline("This should be strong."),
            });

            var expectedResult = @"\begin{paragraph}{}
This is a text.\textbf{This should be strong.}
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnHyperlink()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("There is some text and a "),
                new HyperlinkInline(
                text: "link", 
                href: "http://example.com", 
                title: "It is a title"
            )});

            var expectedResult = @"\begin{paragraph}{}
There is some text and a \href{http://example.com}{link}
\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnImage()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TextParagraphBlock(new []
            {
                new ImageInline(alt: "Alt", src: @"C:\Path\To\An\Image.jpg", title: "It has a title too"),
            });


            var expectedResult = @"\begin{paragraph}{}
\begin{figure}[h]
\centering
\includegraphics{C:\Path\To\An\Image.jpg}
\caption{It has a title too}
\end{figure}

\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }
        
        [Fact]
        public void LatexFormatter_ShouldReturnCode()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
            {
                new CleanTextInline("This is a text. "),
                new CodeTextInline("This should be some code."),
            });

            var expectedResult = @"\begin{paragraph}{}
This is a text. \begin{verbatim}
This should be some code.
\end{verbatim}

\end{paragraph}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void LatexFormatter_ShouldReturnEnumeration()
        {
            var latexFormatter = new LatexFormatter();

            var ast = new EnumerationBlock(
                items: new[]
                    {
                        new EnumerationItem(1, new[] { new CleanTextInline("This should be item 1.") }),
                        new EnumerationItem(2, new[] { new CleanTextInline("This should be the second item.")}),
                        new EnumerationItem(1234, new[] { new CleanTextInline("Though, this should be item 3.")})
                    }
                );

            var expectedResult = @"\begin{itemize}
\item [1] This should be item 1.
\item [2] This should be the second item.
\item [1234] Though, this should be item 3.
\end{itemize}
";

            var memoryStream = new MemoryStream();
            latexFormatter.Transform(ast, memoryStream);

            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var result = streamReader.ReadToEnd();
                Assert.Equal(expectedResult, result);
            }
        }


    }
}
