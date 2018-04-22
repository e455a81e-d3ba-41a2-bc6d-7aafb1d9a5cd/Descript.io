using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class BlockquoteBlock : IAbstractSyntaxTreeBlock, IEquatable<BlockquoteBlock>
    {
        public BlockquoteBlock(IImmutableList<IAbstractSyntaxTreeInline> inlines, IAbstractSyntaxTreeBlock next = null)
        {
            Inlines = inlines ?? throw new ArgumentNullException(nameof(inlines));
            Next = next;
        }

        public BlockquoteBlock(IEnumerable<IAbstractSyntaxTreeInline> inlines, IAbstractSyntaxTreeBlock next = null)
        {
            Inlines = ImmutableList.CreateRange(inlines ?? throw new ArgumentNullException(nameof(inlines)));
            Next = next;
        }

        public IAbstractSyntaxTreeBlock Next { get; }

        public IImmutableList<IAbstractSyntaxTreeInline> Inlines { get; }

        public virtual IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next)
            => new BlockquoteBlock(Inlines, next);

        public virtual BlockquoteBlock SetChildBlocks(IImmutableList<IAbstractSyntaxTreeInline> inlines)
            => new BlockquoteBlock(inlines, Next);

        public virtual void Accept(IAbstractSyntaxTreeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public virtual bool Equals(BlockquoteBlock other)
            => ReferenceEquals(this, other)
               || !(other is null)
               && Equals(Next, other.Next)
               && Inlines.IsEquivalentTo(other.Inlines);

        public override bool Equals(object obj) => obj is BlockquoteBlock other && Equals(other);

        public override int GetHashCode()
            => unchecked(((Next?.GetHashCode() ?? 0) * 397) ^ (Inlines?.GetHashCode() ?? 0));
    }
}
