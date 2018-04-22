using System.IO;
using System.Text;
using Descriptio.Core.AST;

namespace Descriptio.Transform.Json
{
    public class JsonFormatter : IFormatter
    {
        public void Transform(IAbstractSyntaxTreeBlock abstractSyntaxTreeBlock, Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                var visitor = new JsonAbstractSyntaxTreeVisitor(streamWriter);
                var block = abstractSyntaxTreeBlock;
                
                while (block != null)
                {
                    block.Accept(visitor);
                    block = block.Next;
                }
                streamWriter.Flush();
            }
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
