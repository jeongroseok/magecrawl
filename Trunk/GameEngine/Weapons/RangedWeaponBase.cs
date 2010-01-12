using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    abstract internal class RangedWeaponBase : WeaponBase, Item
    {
        public override bool IsRanged
        {
            get
            {
                return true;
            }
        }

        protected List<EffectivePoint> GenerateRangedTargetablePoints(int range, int minDistance, int falloffDistance, float falloffPerSquare)
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
                        targetablePoints.Add(new EffectivePoint(new Point(Owner.Position.X + i, Owner.Position.Y + j), weaponStrength));
                    }
                }
            }
            return targetablePoints;
        }
    }
}
