using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;
using Magecrawl.Items.Interfaces;

namespace Magecrawl.Items.WeaponRanges
{
    internal class Dagger : IWeaponRange, IWeaponVerb
    {
        public List<EffectivePoint> CalculateTargetablePoints(IWeapon weapon, Point wielderPosition)
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(1, 1), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(-1, -1), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(-1, 1), .75f));
            targetablePoints.Add(new EffectivePoint(wielderPosition + new Point(1, -1), .75f));
        
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
                return "Dagger";
            }
        }
    }
}
