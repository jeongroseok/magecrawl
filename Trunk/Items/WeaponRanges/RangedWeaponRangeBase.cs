using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.Interfaces;
using System.Globalization;

namespace Magecrawl.Items.WeaponRanges
{
    internal abstract class RangedWeaponRangeBase : IWeaponRange
    {
        public bool IsRanged
        {
            get
            {
                return true;
            }
        }
        
        public abstract string Name { get; }

        public List<EffectivePoint> CalculateTargetablePoints(IWeapon weapon, Point wielderPosition)
        {
            Weapon weaponCore = (Weapon)weapon;
            int baseRange = int.Parse(weaponCore.Attributes["BaseRange"], CultureInfo.InvariantCulture);
            int baseMinRange = int.Parse(weaponCore.Attributes["BaseMinRange"], CultureInfo.InvariantCulture);
            int baseFalloffStart = int.Parse(weaponCore.Attributes["BaseFalloffStart"], CultureInfo.InvariantCulture);
            float falloffAmount = float.Parse(weaponCore.Attributes["FalloffAmount"], CultureInfo.InvariantCulture);

            List<EffectivePoint> targetablePoints = GenerateRangedTargetablePoints(wielderPosition,
                baseRange, baseMinRange, baseFalloffStart, falloffAmount);

            return targetablePoints;
        }

        private static List<EffectivePoint> GenerateRangedTargetablePoints(Point wielderPosition, int range, int minDistance, int falloffDistance, float falloffPerSquare)
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
