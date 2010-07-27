using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.Utilities
{
    public static class ListUtils
    {
        public static List<T> TrimToLength<T>(this List<T> list, int length)
        {
            if (list.Count <= length)
                return list;
            list.RemoveRange(length, list.Count - length);
            return list;
        }
    }
}
