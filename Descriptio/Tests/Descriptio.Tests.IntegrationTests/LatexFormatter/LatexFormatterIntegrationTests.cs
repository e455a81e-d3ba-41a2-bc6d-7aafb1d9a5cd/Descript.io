using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Descriptio.Core.AST;
using Xunit;

namespace Descriptio.Tests.IntegrationTests.LatexFormatter
{
    public class LatexFormatterIntegrationTests
    {
        [Fact]
        public void LatexFormatter_ShouldYieldDocument()
        {
            var latexFormatter = new Descriptio.Transform.Latex.LatexFormatter();

            var ast = new TitleAst(
                "Title 1",
                level: 1,
                next: new TextParagraphBlock(new[]
                    {
                        new CleanTextInline("This is a text."),
                    },
                    next: new TitleAst(
                        "Title 2",
                        level: 2,
                        next: new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
                        {
                            new CleanTextInline("This is another text. "),
                            new StrongTextInline("This should be strong."),
                            new CleanTextInline(" "),
                            new EmphasisTextInline("And this should be emphasized."),
                            new CleanTextInline(" "),
                            new CodeTextInline("This should be formatted as code."),
                        },
                            next: new TextParagraphBlock(new IAbstractSyntaxTreeInline[]
                            {
                                new CleanTextInline("Here, we should have a new paragraph "),
                                new HyperlinkInline(text: "with a link", href: "http://example.com", title: "It is a title"),
                                new CleanTextInline("."),
                                new ImageInline(alt: "Alt", src: @"C:\Path\To\An\Image.jpg", title: "It has a title too"),
                            },
                                next: new EnumerationBlock(
                                    items: new[]
                                    {
                                        new EnumerationItem(1, new[] { new CleanTextInline("This should be item 1.") }),
                                        new EnumerationItem(2, new[] { new CleanTextInline("This should be the second item.")}),
                                        new EnumerationItem(3, new[] { new CleanTextInline("Though, this should be item 3.")})
                                    }
                                )
                            )
                        )
                )));

            var expectedResult = @"\section*{Title 1}
\begin{paragraph}{}
This is a text.
\end{paragraph}
\section*{Title 2}
\begin{paragraph}{}
This is another text. \textbf{This should be strong.} \emph{And this should be emphasized.} \begin{verbatim}
This should be formatted as code.
\end{verbatim}

\end{paragraph}
\begin{paragraph}{}
Here, we should have a new paragraph \href{http://example.com}{with a link}.\begin{figure}[h]
\centering
\includegraphics{C:\Path\To\An\Image.jpg}
\caption{It has a title too}
\end{figure}

\end{paragraph}
\begin{itemize}
\item [1] This should be item 1.
\item [2] This should be the second item.
\item [3] Though, this should be item 3.
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
