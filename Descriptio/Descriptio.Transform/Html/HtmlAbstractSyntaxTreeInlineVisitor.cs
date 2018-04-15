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
        
        public void Visit(TextInline textInline)
        {
            _streamWriter.Write(textInline.Text);
        }
        
        public void Visit(EmphasisTextInline emphasisTextInline)
        {
            _streamWriter.Write($"<em>{emphasisTextInline.Text}</em>");
        }
        
        public void Visit(StrongTextInline strongTextInline)
        {
            _streamWriter.Write($"<strong>{strongTextInline.Text}</strong>");
        }
        
        public void Visit(CodeTextInline codeTextInline)
        {
            _streamWriter.Write($"<code>{codeTextInline.Text}</code>");
        }
        
        public void Visit(HyperlinkInline hyperlinkInline)
        {
            _streamWriter.Write($"<a href=\"{hyperlinkInline.Href}\">");
            _streamWriter.Write(hyperlinkInline.Text);
            _streamWriter.Write(@"</a>");

        }
        
        public void Visit(ImageInline imageInline)
        {
            _streamWriter.WriteLine(@"<figure>");
            _streamWriter.WriteLine($"<img src=\"{imageInline.Src}\" alt=\"{imageInline.Alt}\"/>");
            _streamWriter.WriteLine($"<figcaption>{imageInline.Title}</figcaption>");
            _streamWriter.WriteLine(@"</figure>");
        }
    }
}
