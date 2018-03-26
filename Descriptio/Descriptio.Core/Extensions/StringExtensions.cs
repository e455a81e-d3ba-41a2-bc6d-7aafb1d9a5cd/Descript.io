using System;

namespace Descriptio.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ThrowIfNullOrEmpty(this string text, Exception exception)
            => string.IsNullOrEmpty(text) ? throw exception : text;

        public static string ThrowIfNullOrEmpty(this string text, string paramName)
            => string.IsNullOrEmpty(text) ? throw new ArgumentNullException(paramName) : text;
    }
}
