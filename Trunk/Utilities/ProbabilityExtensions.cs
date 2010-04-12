using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;

namespace Magecrawl.Utilities
{
    public static class ProbabilityExtensions
    {
        public static bool Chance(this TCODRandom r, int probability)
        {
            return r.getInt(0, 99) < probability;
        }

        public static List<T> Randomize<T>(this List<T> list)
        {
            return list.OrderBy(a => Guid.NewGuid()).ToList();
        }
    }
}

