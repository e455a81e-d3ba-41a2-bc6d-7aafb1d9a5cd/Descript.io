using Microsoft.FSharp.Core;

namespace Descriptio.Tests.FluentAssertionsExtensions
{
    public static class FSharpOptionExtensions
    { 
        public static FSharpOptionAssertions<T> Should<T>(this FSharpOption<T> self)
            => new FSharpOptionAssertions<T>(self);
    }
}
