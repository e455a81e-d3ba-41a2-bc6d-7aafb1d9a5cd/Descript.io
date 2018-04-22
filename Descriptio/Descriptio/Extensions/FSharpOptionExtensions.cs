using Microsoft.FSharp.Core;

namespace Descriptio.Extensions
{
    public static class FSharpOptionExtensions
    {
        public static bool IsSome<T>(this FSharpOption<T> value) => !value.IsNone();
        public static bool IsNone<T>(this FSharpOption<T> value) => value is null || value.Equals(FSharpOption<T>.None);
    }
}
