using System;
using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public static class PointListUtils
    {
        public static List<EffectivePoint> PointListFromBurstPosition(Point center, int burstDistance)
        {
            List<EffectivePoint> returnValue = new List<EffectivePoint>();
            for (int i = -burstDistance; i <= burstDistance; ++i)
            {
                for (int j = -burstDistance; j <= burstDistance; ++j)
                {
                    int distanceFromCenter = System.Math.Abs(i) + Math.Abs(j);
                    if (distanceFromCenter < burstDistance)
                    {
                        returnValue.Add(new EffectivePoint(new Point(center.X + i, center.Y + j), 1.0f));
                    }
                }
            }
            return returnValue;
        }
    }
}
