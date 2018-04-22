using System;
using System.Collections.Generic;
using System.Xml;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Xml
{
    public class XmlAbstractSyntaxTreeVisitor : IAbstractSyntaxTreeVisitor
    {
        private readonly XmlWriter _xmlWriter;
        private readonly XmlAbstractSyntaxTreeInlineVisitor _inlineVisitor;

        public XmlAbstractSyntaxTreeVisitor(XmlWriter xmlWriter)
        {
            _xmlWriter = xmlWriter;
            _inlineVisitor = new XmlAbstractSyntaxTreeInlineVisitor(xmlWriter);
        }

        public void Visit(TitleAst titleAst)
        {
            _xmlWriter.WriteStartElement("title");
            _xmlWriter.WriteValue(titleAst.Text);
            _xmlWriter.WriteEndElement();
        }

        public void Visit(TextParagraphBlock textParagraphBlock)
        {
            _xmlWriter.WriteStartElement("text_paragraph");
            FormatInlines(textParagraphBlock.Inlines);
            _xmlWriter.WriteEndElement();
        }

        public void Visit(EnumerationBlock enumerationBlock)
        {
            _xmlWriter.WriteStartElement("enumeration");
            foreach (var item in enumerationBlock.Items)
            {
                _xmlWriter.WriteStartElement("item");

                _xmlWriter.WriteStartAttribute("number");
                _xmlWriter.WriteValue(item.Number);
                _xmlWriter.WriteEndAttribute();

                FormatInlines(item.Inlines);
                _xmlWriter.WriteEndElement();
            }
            _xmlWriter.WriteEndElement();
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
