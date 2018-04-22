using System;
using System.IO;
using Descriptio.Core.AST;
using Descriptio.Extensions;
using Descriptio.Factories;
using Descriptio.Transform;
using Microsoft.FSharp.Core;

namespace Descriptio
{
    public class ParseResult
    {
        private readonly FormatterFactory _formatterFactory = new FormatterFactory();

        public ParseResult(FSharpOption<IAbstractSyntaxTreeBlock> parserResult)
        {
            ParserResult = parserResult ?? throw new ArgumentNullException(nameof(parserResult));
        }

        public FSharpOption<IAbstractSyntaxTreeBlock> ParserResult { get; }

        public string AndFormatToLaTexString() => FromStream(_formatterFactory.CreateLaTexFormatterWithDefaultRules());

        public string AndFormatToHtmlString() => FromStream(_formatterFactory.CreateHtmlFormatterWithDefaultRules());

        public string AndFormatToXmlString() => FromStream(_formatterFactory.CreateXmlFormatterWithDefaultRules());

        public string AndFormatToStringUsing(IFormatter formatter) => FromStream(formatter ?? throw new ArgumentNullException(nameof(formatter)));

        protected string FromStream(IFormatter formatter)
        {
            ThrowIfCannotFormat();

            using (var targetStream = new MemoryStream())
            {
                formatter.Transform(ParserResult.Value, targetStream);

                targetStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(targetStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        protected void ThrowIfCannotFormat()
        {
            if (ParserResult.IsNone())
            {
                throw new InvalidOperationException(
                    "Formatting cannot be executed, because parsing failed previously.");
            }
        }
    }
}
