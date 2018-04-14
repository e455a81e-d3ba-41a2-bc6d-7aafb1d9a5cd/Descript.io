using System;
using System.Collections.Immutable;

namespace Descriptio.Core.AST
{
    public class BlockquoteBlock : IAbstractSyntaxTreeBlock
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
    }
}
