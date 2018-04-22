using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class UnorderedEnumerationItem : IAbstractSyntaxTree, IEquatable<UnorderedEnumerationItem>
    {
        public UnorderedEnumerationItem(int indent, char bullet, IImmutableList<IAbstractSyntaxTreeInline> inlines)
        {
            Indent = indent < 0 ? throw new ArgumentOutOfRangeException(nameof(indent)) : indent;
            Bullet = bullet;
            Inlines = inlines ?? throw new ArgumentNullException(nameof(inlines));
        }

        public UnorderedEnumerationItem(int indent, char bullet, IEnumerable<IAbstractSyntaxTreeInline> inlines)
            : this(indent, bullet, ImmutableList.CreateRange(inlines ?? throw new ArgumentNullException(nameof(inlines))))
        {
        }

        public int Indent { get; }

        public char Bullet { get; }

        public IImmutableList<IAbstractSyntaxTreeInline> Inlines { get; }

        public bool Equals(UnorderedEnumerationItem other)
            => ReferenceEquals(this, other)
               || !(other is null)
               && Indent == other.Indent
               && Bullet == other.Bullet
               && Inlines.IsEquivalentTo(other.Inlines);

        public override bool Equals(object obj) => obj is UnorderedEnumerationItem other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Indent;
                hashCode = (hashCode * 397) ^ Bullet.GetHashCode();
                hashCode = (hashCode * 397) ^ (Inlines != null ? Inlines.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
