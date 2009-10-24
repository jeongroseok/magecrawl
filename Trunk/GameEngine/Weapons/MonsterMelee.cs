using System;
using System.Collections.Generic;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Weapons
{
    internal class MonsterMelee : WeaponBase
    {
        internal MonsterMelee()
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
                return "Monster Melee";
            }
        }

        public override List<WeaponPoint> TargetablePoints(Point characterPosition)
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(0, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(0, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(-1, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterPosition + new Point(1, -1), 1.0f));

            StripImpossibleToTargetPoints(targetablePoints);

            return targetablePoints;
        }
    }
}
