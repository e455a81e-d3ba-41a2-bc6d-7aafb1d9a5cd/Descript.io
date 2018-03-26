using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class EmphasisTextInline : TextInline
    {
        public EmphasisTextInline(string text) : base(text)
        {
        }

        public override TextInline SetText(string text)
            => new EmphasisTextInline(text.ThrowIfNullOrEmpty(nameof(text)));
    }
}
