using System;

namespace Accounting.Common
{
    public static class StringExtensions
    {
        public static string RemoveFromEnd(this string str, string value) =>
            str.EndsWith(value, StringComparison.OrdinalIgnoreCase)
                ? str.Substring(0, str.Length - value.Length)
                : str;
    }
}