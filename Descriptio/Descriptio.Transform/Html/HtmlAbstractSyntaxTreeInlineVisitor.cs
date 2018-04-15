using System;
using System.IO;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Html
{
    public class HtmlAbstractSyntaxTreeInlineVisitor : IAbstractSyntaxTreeInlineVisitor 
    {
        private readonly StreamWriter _streamWriter;

        public HtmlAbstractSyntaxTreeInlineVisitor(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void Visit(CleanTextInline cleanTextInline)
        {
            _streamWriter.WriteLine(cleanTextInline.Text);
        }

        public void Visit(CodeTextInline codeTextInline)
        {
            throw new NotImplementedException();
        }

        public void Visit(EmphasisTextInline emphasisTextInline)
        {
            throw new NotImplementedException();
        }

        public void Visit(HyperlinkInline hyperlinkInline)
        {
            throw new NotImplementedException();
        }

        public void Visit(ImageInline imageInline)
        {
            throw new NotImplementedException();
        }

        public void Visit(StrongTextInline strongTextInline)
        {
            throw new NotImplementedException();
        }

        public void Visit(TextInline textInline)
        {
            throw new NotImplementedException();
        }
    }
}
