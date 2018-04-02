using System.Collections.Generic;
using System.Linq;

namespace Descriptio.Core.Extensions
{
    public static class ReadOnlyListExtensions
    {
        public static bool IsEquivalentTo<T>(this IReadOnlyList<T> self, IReadOnlyList<T> other)
            => ReferenceEquals(self, other)
               || !(self is null)
               && !(other is null)
               && self.Count == other.Count
               && !self.Where((t, n) => !t.Equals(other[n])).Any();
    }
}
