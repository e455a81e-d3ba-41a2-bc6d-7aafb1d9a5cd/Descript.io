using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Latex
{
    public class LatexAbstractSyntaxTreeVisitor : IAbstractSyntaxTreeVisitor
    {
        private readonly StreamWriter _streamWriter;
        private readonly LatexAbstractSyntaxTreeInlineVisitor _inlineVisitor;

        public LatexAbstractSyntaxTreeVisitor(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
            _inlineVisitor = new LatexAbstractSyntaxTreeInlineVisitor(_streamWriter);
        }

        public void Visit(TitleAst titleAst)
        {
            _streamWriter.WriteLine($"\\section*{{{titleAst.Text}}}");
        }

        public void Visit(TextParagraphBlock textParagraphBlock)
        {
            _streamWriter.WriteLine(@"\begin{paragraph}{}");
            FormatInlines(textParagraphBlock.Inlines);
            _streamWriter.WriteLine(@"\end{paragraph}");
        }

        public void Visit(EnumerationBlock enumerationBlock)
        {
            _streamWriter.WriteLine(@"\begin{itemize}");
            foreach (var item in enumerationBlock.Items)
            {
                _streamWriter.Write($"\\item [{item.Number}] ");
                FormatInlines(item.Inlines);
            }
            _streamWriter.WriteLine(@"\end{itemize}");
        }

        private void FormatInlines(IEnumerable<IAbstractSyntaxTreeInline> inlines)
        {
            foreach (var inline in inlines)
            {
                inline.Accept(_inlineVisitor);
            }
            _streamWriter.WriteLine();
        }
    }
}
