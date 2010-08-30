using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;

namespace Magecrawl.Utilities
{
    public static class ProbabilityExtensions
    {
        public static bool Chance(this Random r, int probability)
        {
            return r.Next(100) < probability;
        }

        public static bool Chance(this Random r, double probability)
        {
            return r.NextDouble() < probability;
        }

        public static List<T> Randomize<T>(this List<T> list)
        {
            return list.OrderBy(a => Guid.NewGuid()).ToList();
        }

        public static int getInt(this Random r, int min, int inclusiveMax)
        {
            return min + r.Next(inclusiveMax - min + 1);
        }
    }
}

