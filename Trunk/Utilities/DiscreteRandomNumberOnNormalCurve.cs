using System;
using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public static class DiscreteRandomNumberOnNormalCurve
    {
        private static Random s_random = new Random();

        public static int GetNumber(double mean, double var, int low, int high)
        {
            NormalDistribution n = new NormalDistribution(mean, var);
            while (true)
            {
                int attempt = s_random.getInt(low, high);
                double chanceAtPosition = n.PDF(attempt);
                if (s_random.Chance(chanceAtPosition))
                    return attempt;
            }
        }

        public static void ManualTest(double mean, double var, int low, int high)
        {
            Dictionary<int, int> results = new Dictionary<int, int>();
            for (int i = low; i <= high; ++i)
                results[i] = 0;

            for (int i = 0; i < 1000000; ++i)
            {
                results[GetNumber(mean, var, low, high)]++;
            }
        }
    }
}
