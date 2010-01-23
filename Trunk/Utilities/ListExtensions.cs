using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public static class ListExtensions
    {
        public static T EndElement<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }
    }
}
