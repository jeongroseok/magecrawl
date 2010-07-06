using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal static class RangedWeaponCalculator
    {
        internal static List<EffectivePoint> GenerateRangedTargetablePoints(Point wielderPosition, int range, int minDistance, int falloffDistance, float falloffPerSquare)
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            for (int i = -range; i <= range; ++i)
            {
                for (int j = -range; j <= range; ++j)
                {
                    int distance = System.Math.Abs(i) + Math.Abs(j);
                    if ((distance <= range) && (distance >= minDistance))
                    {
                        float weaponStrength = 1.0f - (Math.Max(distance - falloffDistance, 0) * falloffPerSquare);
                        targetablePoints.Add(new EffectivePoint(new Point(wielderPosition.X + i, wielderPosition.Y + j), weaponStrength));
                    }
                }
            }
            return targetablePoints;
        }
    }
}
