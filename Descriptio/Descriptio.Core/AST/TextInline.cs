using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public abstract class TextInline : IAbstractSyntaxTreeInline
    {
        protected TextInline(string text)
        {
            Text = text.ThrowIfNullOrEmpty(nameof(text));
        }

        public string Text { get; }

        public abstract TextInline SetText(string text);
    }
}
