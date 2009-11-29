using System;
using libtcodWrapper;

namespace Magecrawl.Utilities
{
    public static class ProbabilityExtensions
    {
        public static bool Chance(this TCODRandom r, int probability)
        {
            return r.GetRandomInt(0, 99) < probability;
        }
    }
}

