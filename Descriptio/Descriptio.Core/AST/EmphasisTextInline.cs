using System;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    [System.Diagnostics.DebuggerDisplay("*{" + nameof(Text) + "}*")]
    public class EmphasisTextInline : TextInline, IEquatable<EmphasisTextInline>
    {
        public EmphasisTextInline(string text) : base(text)
        {
        }

        public override TextInline SetText(string text)
            => new EmphasisTextInline(text.ThrowIfNullOrEmpty(nameof(text)));

        public bool Equals(EmphasisTextInline other) => Text == other?.Text;

        public override bool Equals(object obj) => obj is EmphasisTextInline other && Equals(other);

        public override int GetHashCode() => Text.GetHashCode();

        public override void Accept(IAbstractSyntaxTreeInlineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
