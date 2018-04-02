using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class TextParagraphBlock : IAbstractSyntaxTreeBlock, IEquatable<TextParagraphBlock>
    {
        public TextParagraphBlock(IEnumerable<IAbstractSyntaxTreeInline> inlines, IAbstractSyntaxTreeBlock next = null)
        {
            Next = next;
            Inlines = ImmutableList.CreateRange(inlines ?? throw new ArgumentNullException(nameof(inlines)));
        }

        public TextParagraphBlock(IImmutableList<IAbstractSyntaxTreeInline> inlines, IAbstractSyntaxTreeBlock next = null)
        {
            Next = next;
            Inlines = inlines ?? throw new ArgumentNullException(nameof(inlines));
        }

        public IAbstractSyntaxTreeBlock Next { get; }

        public IImmutableList<IAbstractSyntaxTreeInline> Inlines { get; }

        public IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next) => new TextParagraphBlock(Inlines, next);

        public TextParagraphBlock SetInlines(IImmutableList<IAbstractSyntaxTreeInline> inlines)
            => new TextParagraphBlock(inlines, Next);

        public TextParagraphBlock SetInlines(IEnumerable<IAbstractSyntaxTreeInline> inlines)
            => new TextParagraphBlock(inlines, Next);

        public bool Equals(TextParagraphBlock other)
            => !(other is null) && Inlines.IsEquivalentTo(other.Inlines);

        public override bool Equals(object obj) => obj is TextParagraphBlock tpb && Equals(tpb);

        public override int GetHashCode() => Inlines?.GetHashCode() ?? 0;
    }
}
