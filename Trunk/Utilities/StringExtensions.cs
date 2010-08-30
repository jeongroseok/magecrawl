using System;

namespace Magecrawl.Utilities
{
    public static class StringExtensions
    {
        public static string UpperCaseFirstLetter(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
        
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
