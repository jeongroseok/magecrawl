using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

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

        public override List<Point> TargetablePoints(Point characterPosition)
        {
            List<Point> targetablePoints = new List<Point>();

            targetablePoints.Add(characterPosition + new Point(1, 0));
            targetablePoints.Add(characterPosition + new Point(-1, 0));
            targetablePoints.Add(characterPosition + new Point(0, 1));
            targetablePoints.Add(characterPosition + new Point(0, -1));
            targetablePoints.Add(characterPosition + new Point(1, 1));
            targetablePoints.Add(characterPosition + new Point(-1, -1));
            targetablePoints.Add(characterPosition + new Point(-1, 1));
            targetablePoints.Add(characterPosition + new Point(1, -1));

            StripImpossibleToTargetPoints(targetablePoints);

            return targetablePoints;
        }
    }
}
