using System.IO;
using Descriptio.Core.AST;
using Newtonsoft.Json;

namespace Descriptio.Transform.Json
{
    public class JsonAbstractSyntaxTreeInlineVisitor : IAbstractSyntaxTreeInlineVisitor
    {
        private readonly StreamWriter _streamWriter;

        public JsonAbstractSyntaxTreeInlineVisitor(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void Visit(CleanTextInline cleanTextInline)
        {
            var json = JsonConvert.SerializeObject(cleanTextInline);
            _streamWriter.WriteLine(json);
        }
        
        public void Visit(TextInline textInline)
        {
            var json = JsonConvert.SerializeObject(textInline);
            _streamWriter.WriteLine(json);
        }
        
        public void Visit(EmphasisTextInline emphasisTextInline)
        {
            var json = JsonConvert.SerializeObject(emphasisTextInline);
            _streamWriter.WriteLine(json);
        }
        
        public void Visit(StrongTextInline strongTextInline)
        {
            var json = JsonConvert.SerializeObject(strongTextInline);
            _streamWriter.WriteLine(json);
        }
        
        public void Visit(CodeTextInline codeTextInline)
        {
            var json = JsonConvert.SerializeObject(codeTextInline);
            _streamWriter.WriteLine(json);
        }
        
        public void Visit(HyperlinkInline hyperlinkInline)
        {
            var json = JsonConvert.SerializeObject(hyperlinkInline);
            _streamWriter.WriteLine(json);

        }
        
        public void Visit(ImageInline imageInline)
        {
            var json = JsonConvert.SerializeObject(imageInline);
            _streamWriter.WriteLine(json);
        }
    }
}
