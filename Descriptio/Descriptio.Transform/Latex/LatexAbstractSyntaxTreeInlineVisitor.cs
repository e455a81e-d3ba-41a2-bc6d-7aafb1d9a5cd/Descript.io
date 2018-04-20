using System;
using System.IO;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Latex
{
    public class LatexAbstractSyntaxTreeInlineVisitor : IAbstractSyntaxTreeInlineVisitor 
    {
        private readonly StreamWriter _streamWriter;

        public LatexAbstractSyntaxTreeInlineVisitor(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void Visit(CleanTextInline cleanTextInline)
        {
            _streamWriter.Write(cleanTextInline.Text);
        }

        public void Visit(CodeTextInline codeTextInline)
        {
            _streamWriter.WriteLine("\\begin{verbatim}");
            _streamWriter.WriteLine(codeTextInline.Text);
            _streamWriter.WriteLine("\\end{verbatim}");
        }

        public void Visit(EmphasisTextInline emphasisTextInline)
        {
            _streamWriter.Write($"\\emph{{{emphasisTextInline.Text}}}");
        }

        public void Visit(HyperlinkInline hyperlinkInline)
        {
            _streamWriter.Write($"\\href{{{hyperlinkInline.Href}}}{{{hyperlinkInline.Text}}}");
        }

        public void Visit(ImageInline imageInline)
        {
            _streamWriter.WriteLine("\\begin{figure}[h]");
            _streamWriter.WriteLine("\\centering");
            _streamWriter.WriteLine($"\\includegraphics{{{imageInline.Src}}}");

            if (!string.IsNullOrEmpty(imageInline.Title))
                _streamWriter.WriteLine($"\\caption{{{imageInline.Title}}}");

            _streamWriter.WriteLine("\\end{figure}");
        }

        public void Visit(StrongTextInline strongTextInline)
        {
            _streamWriter.Write($"\\textbf{{{strongTextInline.Text}}}");
        }

        public void Visit(TextInline textInline)
        {
            _streamWriter.Write(textInline.Text);
        }
    }
}
