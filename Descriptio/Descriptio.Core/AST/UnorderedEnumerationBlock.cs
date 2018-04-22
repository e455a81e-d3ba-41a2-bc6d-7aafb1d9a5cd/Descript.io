using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class UnorderedEnumerationBlock : IAbstractSyntaxTreeBlock, IEquatable<UnorderedEnumerationBlock>
    {
        public UnorderedEnumerationBlock(IImmutableList<UnorderedEnumerationItem> items, IAbstractSyntaxTreeBlock next = null)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Next = next;
        }

        public UnorderedEnumerationBlock(IEnumerable<UnorderedEnumerationItem> items, IAbstractSyntaxTreeBlock next = null)
            : this(ImmutableList.CreateRange(items ?? throw new ArgumentNullException(nameof(items))), next)
        {
        }

        public IImmutableList<UnorderedEnumerationItem> Items { get; }

        public IAbstractSyntaxTreeBlock Next { get; }

        public UnorderedEnumerationBlock SetItems(IImmutableList<UnorderedEnumerationItem> items)
            => new UnorderedEnumerationBlock(items, Next);

        public IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next)
            => new UnorderedEnumerationBlock(Items, next);

        public void Accept(IAbstractSyntaxTreeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public bool Equals(UnorderedEnumerationBlock other)
            => ReferenceEquals(this, other)
               || !(other is null)
               && Items.IsEquivalentTo(other.Items)
               && Equals(Next, other.Next);

        public override bool Equals(object obj) => obj is UnorderedEnumerationBlock other && Equals(other);

        public override int GetHashCode()
            => unchecked(((Items != null ? Items.GetHashCode() : 0) * 397) ^ (Next != null ? Next.GetHashCode() : 0));
    }
}
