using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Html
{
    public class HtmlAbstractSyntaxTreeVisitor : IAbstractSyntaxTreeVisitor
    {
        private readonly StreamWriter _streamWriter;
        private readonly HtmlAbstractSyntaxTreeInlineVisitor _inlineVisitor;

        public HtmlAbstractSyntaxTreeVisitor(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
            _inlineVisitor = new HtmlAbstractSyntaxTreeInlineVisitor(_streamWriter);
        }

        public void Visit(TitleAst titleAst)
        {
            _streamWriter.Write($"<h1>{titleAst.Text}</h1>");
        }

        public void Visit(TextParagraphBlock textParagraphBlock)
        {
            _streamWriter.WriteLine(@"<p>");
            FormatInlines(textParagraphBlock.Inlines);
            _streamWriter.Write("</p>");
        }

        public void Visit(EnumerationBlock enumerationBlock)
        {
            _streamWriter.WriteLine(@"<ol>");
            foreach (var item in enumerationBlock.Items)
            {
                _streamWriter.WriteLine(@"<li>");
                FormatInlines(item.Inlines);
                _streamWriter.WriteLine(@"</li>");
            }
            _streamWriter.WriteLine(@"</ol>");
        }

        private void FormatInlines(IEnumerable<IAbstractSyntaxTreeInline> inlines)
        {
            foreach (var inline in inlines)
            {
                inline.Accept(_inlineVisitor);
            }
        }
    }
}
