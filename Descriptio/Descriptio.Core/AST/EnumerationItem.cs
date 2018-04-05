using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class EnumerationItem : IAbstractSyntaxTree, IEquatable<EnumerationItem>
    {
        public EnumerationItem(int number, IImmutableList<IAbstractSyntaxTreeInline> inlines)
        {
            Inlines = inlines ?? throw new ArgumentNullException(nameof(inlines));
            Number = number;
        }

        public EnumerationItem(int number, IEnumerable<IAbstractSyntaxTreeInline> inlines)
        {
            Inlines = ImmutableList.CreateRange(inlines ?? throw new ArgumentNullException(nameof(inlines)));
            Number = number;
        }

        public IImmutableList<IAbstractSyntaxTreeInline> Inlines { get; }

        /// <remarks>
        ///     This property is currently not relevant for enumeration ordering in markdown,
        ///     but may be interesting for transforms to other languages like HTML,
        ///     or transforming back to Markdown.
        /// </remarks>
        public int Number { get; }

        public virtual EnumerationItem SetInlines(IImmutableList<IAbstractSyntaxTreeInline> inlines)
            => new EnumerationItem(Number, inlines);

        public virtual EnumerationItem SetNumber(int number)
            => new EnumerationItem(number, Inlines);

        public bool Equals(EnumerationItem other)
        {
            if (ReferenceEquals(this, other)) return true;
            return ReferenceEquals(this, other)
                   || !(other is null)
                   && Inlines.IsEquivalentTo(other.Inlines)
                   && Number == other.Number;
        }

        public override bool Equals(object obj) => obj is EnumerationItem other && Equals(other);

        public override int GetHashCode() => unchecked(((Inlines != null ? Inlines.GetHashCode() : 0) * 397) ^ Number);
    }
}
