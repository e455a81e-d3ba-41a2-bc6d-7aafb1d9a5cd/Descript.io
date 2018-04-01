using Microsoft.FSharp.Core;

namespace Descriptio.Tests.UnitTests.Extensions
{
    public static class FluentAssertionsExtensions
    { 
        public static FSharpOptionAssertions<T> Should<T>(this FSharpOption<T> self)
            => new FSharpOptionAssertions<T>(self);
    }
}
