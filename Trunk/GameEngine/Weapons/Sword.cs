using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class Sword : WeaponBase
    {
        internal Sword()
        {
        }

        public override DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(1, 8);
            }
        }

        public override string Name
        {
            get 
            {
                return "Sword";
            }
        }

        public override List<WeaponPoint> TargetablePoints(Point characterPosition)
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(0, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(0, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, 1), .75f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, -1), .75f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, 1), .75f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, -1), .75f));
            
            StripImpossibleToTargetPoints(targetablePoints);

            return targetablePoints;
        }
    }
}
