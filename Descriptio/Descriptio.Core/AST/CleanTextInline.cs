using System;

namespace Descriptio.Core.AST
{
    public class CleanTextInline : TextInline, IEquatable<CleanTextInline>
    {
        public CleanTextInline(string text) : base(text)
        {
        }

        public override TextInline SetText(string newText) => new CleanTextInline(newText);

        public virtual bool Equals(CleanTextInline other) => !(other is null) && other.Text == Text;

        public override bool Equals(object obj) => obj is CleanTextInline o && Equals(o);

        public override int GetHashCode() => Text?.GetHashCode() ?? 0;
    }
}
