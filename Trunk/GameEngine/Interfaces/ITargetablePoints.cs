using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
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
            foreach (EffectivePoint t in targetablePoints)
            {
                if (t.Position == pointOfInterest)
                    return true;
            }
            return false;
        }
    }

    public interface ITargetablePoints
    {
        List<EffectivePoint> CalculateTargetablePoints();
    }
}
