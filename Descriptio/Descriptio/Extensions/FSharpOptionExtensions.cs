using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.FSharp.Core;

namespace Descriptio.Extensions
{
    public static class FSharpOptionExtensions
    {
        public static bool IsSome<T>(this FSharpOption<T> value) => !value.Equals(FSharpOption<T>.None);
        public static bool IsNone<T>(this FSharpOption<T> value) => value.Equals(FSharpOption<T>.None);
    }
}
