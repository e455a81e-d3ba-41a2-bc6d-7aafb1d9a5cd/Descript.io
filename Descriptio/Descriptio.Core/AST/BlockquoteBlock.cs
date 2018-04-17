using System;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class BlockquoteBlock : IAbstractSyntaxTreeBlock, IEquatable<BlockquoteBlock>
    {
        public BlockquoteBlock(IImmutableList<IAbstractSyntaxTreeBlock> childBlocks, IAbstractSyntaxTreeBlock next)
        {
            ChildBlocks = childBlocks ?? throw new ArgumentNullException(nameof(childBlocks));
            Next = next;
        }

        public IAbstractSyntaxTreeBlock Next { get; }

        public IImmutableList<IAbstractSyntaxTreeBlock> ChildBlocks { get; }

        public virtual IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next)
            => new BlockquoteBlock(ChildBlocks, next);

        public virtual BlockquoteBlock SetChildBlocks(IImmutableList<IAbstractSyntaxTreeBlock> blocks)
            => new BlockquoteBlock(blocks, Next);

        public virtual void Accept(IAbstractSyntaxTreeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public virtual bool Equals(BlockquoteBlock other)
            => ReferenceEquals(this, other)
               || !ReferenceEquals(null, other)
               && Equals(Next, other.Next)
               && ChildBlocks.IsEquivalentTo(other.ChildBlocks);

        public override bool Equals(object obj) => obj is BlockquoteBlock other && Equals(other);

        public override int GetHashCode()
            => unchecked(((Next?.GetHashCode() ?? 0) * 397) ^ (ChildBlocks?.GetHashCode() ?? 0));
    }
}
