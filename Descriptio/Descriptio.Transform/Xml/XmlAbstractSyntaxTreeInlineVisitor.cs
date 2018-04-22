using System.Xml;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Xml
{
    public class XmlAbstractSyntaxTreeInlineVisitor : IAbstractSyntaxTreeInlineVisitor
    {
        private readonly XmlWriter _xmlWriter;

        public XmlAbstractSyntaxTreeInlineVisitor(XmlWriter xmlWriter)
        {
            _xmlWriter = xmlWriter;
        }

        public void Visit(CleanTextInline cleanTextInline)
        {
            _xmlWriter.WriteStartElement("clean_text");
            _xmlWriter.WriteValue(cleanTextInline.Text);
            _xmlWriter.WriteEndElement();
        }

        public void Visit(CodeTextInline codeTextInline)
        {
            _xmlWriter.WriteStartElement("code_text");
            _xmlWriter.WriteValue(codeTextInline.Text);
            _xmlWriter.WriteEndElement();
        }

        public void Visit(EmphasisTextInline emphasisTextInline)
        {
            _xmlWriter.WriteStartElement("emphasised_text");
            _xmlWriter.WriteValue(emphasisTextInline.Text);
            _xmlWriter.WriteEndElement();
        }

        public void Visit(HyperlinkInline hyperlinkInline)
        {
            _xmlWriter.WriteStartElement("hyperlink");

            _xmlWriter.WriteStartElement("hyperlink_text");
            _xmlWriter.WriteValue(hyperlinkInline.Text);
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteStartElement("hyperlink_href");
            _xmlWriter.WriteValue(hyperlinkInline.Href);
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteEndElement();
        }

        public void Visit(ImageInline imageInline)
        {
            _xmlWriter.WriteStartElement("image");

            _xmlWriter.WriteStartElement("image_src");
            _xmlWriter.WriteValue(imageInline.Src);
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteStartElement("image_alt");
            _xmlWriter.WriteValue(imageInline.Alt);
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteStartElement("image_title");
            _xmlWriter.WriteValue(imageInline.Title);
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteEndElement();
        }

        public void Visit(StrongTextInline strongTextInline)
        {
            _xmlWriter.WriteStartElement("strong_text");
            _xmlWriter.WriteValue(strongTextInline.Text);
            _xmlWriter.WriteEndElement();
        }

        public void Visit(TextInline textInline)
        {
            _xmlWriter.WriteStartElement("text");
            _xmlWriter.WriteValue(textInline.Text);
            _xmlWriter.WriteEndElement();
        }
    }
}
