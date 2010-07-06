using System.Collections.Generic;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal class Sling : IWeaponRange, IWeaponVerb
    {
        public List<EffectivePoint> CalculateTargetablePoints(Point wielderPosition)
        {
            const int SimpleSlingRange = 5;
            const int SimpleSlingMinRange = 2;
            const int SimpleSlingFalloffStart = SimpleSlingRange;
            const float SimpleSlingFalloffAmount = 0;

            List<EffectivePoint> targetablePoints = RangedWeaponCalculator.GenerateRangedTargetablePoints(wielderPosition, SimpleSlingRange, SimpleSlingMinRange, SimpleSlingFalloffStart, SimpleSlingFalloffAmount);

            return targetablePoints;
        }

        public string AttackVerb
        {
            get
            {
                return "slings a stone at";
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
                return "Sling";
            }
        }
    }
}
