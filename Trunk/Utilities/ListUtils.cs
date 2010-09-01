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

#if SILVERLIGHT
        public static List<TOutput> ConvertAll<TInput, TOutput>(this List<TInput> list) where TOutput : class
        {
            if (list == null)
                throw new ArgumentException();

            return list.Select(x => x as TOutput).ToList();
        }

        public static bool Exists<T>(this List<T> list, Func<T, bool> match)
        {
            if (list == null)
                throw new ArgumentException();

            return list.Any(match);
        }

        public static T Find<T>(this List<T> list, Func<T, bool> match)
        {
            if (list == null)
                throw new ArgumentException();

            return list.FirstOrDefault(match);
        }

        public static int RemoveAll<T>(this List<T> data, Predicate<T> test)
        {
            int removed = 0;

            for (int i = data.Count - 1; i >= 0; i--)
            {
                if (test(data[(i)]))
                {
                    data.RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }

#endif
    }
}
