using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal class Spear : IWeaponRange, IWeaponVerb
    {
        public List<EffectivePoint> CalculateTargetablePoints(IWeapon weapon, Point wielderPosition)
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(1, 0), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(2, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(-1, 0), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(-2, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(0, 1), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(0, 2), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(0, -1), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(0, -2), 1.0f));

            return targetablePoints;
        }

        public string AttackVerb
        {
            get
            {
                return "stabs at";
            }
        }

        public bool IsRanged
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return "Spear";
            }
        }
    }
}
