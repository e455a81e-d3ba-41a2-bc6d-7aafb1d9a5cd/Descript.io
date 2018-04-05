using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class EnumerationBlock : IAbstractSyntaxTreeBlock, IEquatable<EnumerationBlock>
    {
        public EnumerationBlock(IImmutableList<EnumerationItem> items, IAbstractSyntaxTreeBlock next = null)
        {
            Items = items is null
                ? throw new ArgumentNullException(nameof(items))
                : items.Count == 0
                    ? throw new ArgumentException("At least one item has to be set.", nameof(items))
                    : items;
            Next = next;
        }

        public EnumerationBlock(IEnumerable<EnumerationItem> items, IAbstractSyntaxTreeBlock next = null) : this(
            ImmutableList.CreateRange(items ?? throw new ArgumentNullException(nameof(items))),
            next)
        {
        }

        public IImmutableList<EnumerationItem> Items { get; }

        public IAbstractSyntaxTreeBlock Next { get; }

        public virtual IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next)
            => new EnumerationBlock(Items, next);

        public virtual bool Equals(EnumerationBlock other) => ReferenceEquals(this, other)
                                                              || !(other is null)
                                                              && Items.IsEquivalentTo(other.Items)
                                                              && Equals(Next, other.Next);

        public virtual EnumerationBlock SetItems(IImmutableList<EnumerationItem> items)
            => new EnumerationBlock(items, Next);

        public override bool Equals(object obj) => obj is EnumerationBlock other && Equals(other);

        public override int GetHashCode()
            => unchecked(((Items != null ? Items.GetHashCode() : 0) * 397) ^ (Next != null ? Next.GetHashCode() : 0));
    }
}
