using System.IO;
using Descriptio.Core.AST;
using Newtonsoft.Json;

namespace Descriptio.Transform.Json
{
    public class JsonAbstractSyntaxTreeVisitor : IAbstractSyntaxTreeVisitor
    {
        private readonly StreamWriter _streamWriter;
        private readonly JsonAbstractSyntaxTreeInlineVisitor _inlineVisitor;

        public JsonAbstractSyntaxTreeVisitor(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
            _inlineVisitor = new JsonAbstractSyntaxTreeInlineVisitor(_streamWriter);
        }

        public void Visit(TitleAst titleAst)
        {
            var json = JsonConvert.SerializeObject(new { Title = titleAst });
            _streamWriter.WriteLine(json);
        }

        public void Visit(TextParagraphBlock textParagraphBlock)
        {
            var json = JsonConvert.SerializeObject(textParagraphBlock);
            _streamWriter.WriteLine(json);
        }

        public void Visit(EnumerationBlock enumerationBlock)
        {
            var json = JsonConvert.SerializeObject(enumerationBlock);
            _streamWriter.WriteLine(json);
        }
    }
}
