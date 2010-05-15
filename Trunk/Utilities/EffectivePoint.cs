using System;
using System.Collections.Generic;
using System.Linq;

namespace Magecrawl.Utilities
{
    public struct EffectivePoint
    {
        public Point Position;

        // On scale of .01 to 1, lower is less damage.
        public float EffectiveStrength;

        public EffectivePoint(Point p, float str)
        {
            Position = p;
            EffectiveStrength = str;
        }

        // This version is faster since we don't have to calculate targetablePoints over and over again.
        public static bool PositionInTargetablePoints(Point pointOfInterest, List<EffectivePoint> targetablePoints)
        {
            return targetablePoints.Where(t => t.Position == pointOfInterest).Count() > 0;
        }
    }
}
