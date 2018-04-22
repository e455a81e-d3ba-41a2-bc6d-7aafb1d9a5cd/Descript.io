using Descriptio.Transform;
using Descriptio.Transform.Html;
using Descriptio.Transform.Json;
using Descriptio.Transform.Latex;
using Descriptio.Transform.Xml;

namespace Descriptio.Factories
{
    public class FormatterFactory
    {
        public IFormatter CreateLaTexFormatterWithDefaultRules() => new LatexFormatter(); 
        public IFormatter CreateHtmlFormatterWithDefaultRules() => new HtmlFormatter(); 
        public IFormatter CreateXmlFormatterWithDefaultRules() => new XmlFormatter();
        public IFormatter CreateJsonFormatterWithDefaultRules() => new JsonFormatter();
    }
}
