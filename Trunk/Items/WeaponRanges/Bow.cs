using System.Collections.Generic;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal class Bow : IWeaponRange, IWeaponVerb
    {
        public List<EffectivePoint> CalculateTargetablePoints(Point wielderPosition)
        {
            const int SimpleBowRange = 10;
            const int SimpleBowMinRange = 3;
            const int SimpleBowFalloffStart = 4;
            const float SimpleBowFalloffAmount = .25f;

            List<EffectivePoint> targetablePoints = RangedWeaponCalculator.GenerateRangedTargetablePoints(wielderPosition, SimpleBowRange, SimpleBowMinRange, SimpleBowFalloffStart, SimpleBowFalloffAmount);

            return targetablePoints;
        }

        public string AttackVerb
        {
            get
            {
                return "shoots an arrow at";
            }
        }

        public bool IsRanged
        {
            get
            {
                return true;
            }
        }
        public string Name
        {
            get
            {
                return "Bow";
            }
        }

    }
}
