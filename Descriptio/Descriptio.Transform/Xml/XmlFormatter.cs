using System.IO;
using System.Xml;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Xml
{
    public class XmlFormatter : IFormatter
    {
        public void Transform(IAbstractSyntaxTreeBlock abstractSyntaxTreeBlock, Stream stream)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("root");
                var visitor = new XmlAbstractSyntaxTreeVisitor(writer);
                var block = abstractSyntaxTreeBlock;
                while (block != null)
                {
                    block.Accept(visitor);
                    block = block.Next;
                }
                writer.WriteEndElement();
                writer.Flush();
            }
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
