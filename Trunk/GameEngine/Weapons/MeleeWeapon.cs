using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class MeleeWeapon : WeaponBase
    {
        internal MeleeWeapon()
        {
        }

        public override DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(1, 2);
            }
        }

        public override string Name
        {
            get 
            {
                return "Melee";
            }
        }

        public override List<WeaponPoint> CalculateTargetablePoints(Point characterPosition)
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(0, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(0, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, 1), .25f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, -1), .25f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, 1), .25f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, -1), .25f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints);

            return targetablePoints;
        }
    }
}
