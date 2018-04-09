using System;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    [System.Diagnostics.DebuggerDisplay("**{" + nameof(Text) + "}**")]
    public class StrongTextInline : TextInline, IEquatable<StrongTextInline>
    {
        public StrongTextInline(string text) : base(text)
        {
        }

        public override TextInline SetText(string text)
            => new StrongTextInline(text.ThrowIfNullOrEmpty(nameof(text)));

        public bool Equals(StrongTextInline other) => Text == other?.Text;

        public override bool Equals(object obj) => obj is StrongTextInline other && Equals(other);

        public override int GetHashCode() => Text.GetHashCode();

        public override void Accept(IAbstractSyntaxTreeInlineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
