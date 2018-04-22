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

        public string AndFormatToLaTexString() => AndFormatToStringUsing(_formatterFactory.CreateLaTexFormatterWithDefaultRules());

        public string AndFormatToHtmlString() => AndFormatToStringUsing(_formatterFactory.CreateHtmlFormatterWithDefaultRules());

        public string AndFormatToXmlString() => AndFormatToStringUsing(_formatterFactory.CreateXmlFormatterWithDefaultRules());

        public string AndFormatToJsonString() => AndFormatToStringUsing(_formatterFactory.CreateJsonFormatterWithDefaultRules());

        public string AndFormatToStringUsing(IFormatter formatter)
        {
            ThrowIfCannotFormat();

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

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
