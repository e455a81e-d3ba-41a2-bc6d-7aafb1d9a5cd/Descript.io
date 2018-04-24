using System;
using System.IO;
using Descriptio.Core.AST;
using Descriptio.Extensions;
using Descriptio.Factories;
using Descriptio.Transform;
using Microsoft.FSharp.Core;

namespace Descriptio
{
    /// <summary>
    /// Represents the result of a parsing process.
    /// </summary>
    public class ParseResult
    {
        private readonly FormatterFactory _formatterFactory = new FormatterFactory();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult"/> class.
        /// </summary>
        /// <param name="parserResult">
        /// The result of the parsing process, encapsulated in an <see cref="FSharpOption{T}"/> instance.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="parserResult"/> is <c>null</c>.
        /// </exception>
        public ParseResult(FSharpOption<IAbstractSyntaxTreeBlock> parserResult)
        {
            ParserResult = parserResult ?? throw new ArgumentNullException(nameof(parserResult));
        }

        public FSharpOption<IAbstractSyntaxTreeBlock> ParserResult { get; }

        /// <summary>
        /// Formats the result to a LaTex string using the default formatter.
        /// </summary>
        /// <returns>The LaTex string.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ParserResult"/> is <see cref="FSharpOption{T}.None"/>.
        /// </exception>
        public FSharpOption<string> AndFormatToLaTexString() => AndFormatToStringUsing(_formatterFactory.CreateLaTexFormatterWithDefaultRules());

        /// <summary>
        /// Formats the result to an HTML string using the default formatter.
        /// </summary>
        /// <returns>The HTML string.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ParserResult"/> is <see cref="FSharpOption{T}.None"/>.
        /// </exception>
        public FSharpOption<string> AndFormatToHtmlString() => AndFormatToStringUsing(_formatterFactory.CreateHtmlFormatterWithDefaultRules());

        /// <summary>
        /// Formats the result to an XML string using the default formatter.
        /// </summary>
        /// <returns>The XML string.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ParserResult"/> is <see cref="FSharpOption{T}.None"/>.
        /// </exception>
        public FSharpOption<string> AndFormatToXmlString() => AndFormatToStringUsing(_formatterFactory.CreateXmlFormatterWithDefaultRules());

        /// <summary>
        /// Formats the result to a JSON string using the default formatter.
        /// </summary>
        /// <returns>The JSON string.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ParserResult"/> is <see cref="FSharpOption{T}.None"/>.
        /// </exception>
        public FSharpOption<string> AndFormatToJsonString() => AndFormatToStringUsing(_formatterFactory.CreateJsonFormatterWithDefaultRules());

        /// <summary>
        /// Formats the result using a specified formatter-
        /// </summary>
        /// <param name="formatter">
        /// The formatter to use.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>The formatted string.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="formatter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ParserResult"/> is <see cref="FSharpOption{T}.None"/>.
        /// </exception>
        public FSharpOption<string> AndFormatToStringUsing(IFormatter formatter)
        {
            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (ParserResult.IsNone())
            {
                return FSharpOption<string>.None;
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
    }
}
