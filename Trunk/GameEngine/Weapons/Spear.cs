using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class Spear : WeaponBase
    {
        internal Spear()
        {
        }

        public override DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(2, 2);
            }
        }

        public override string Name
        {
            get 
            {
                return "Spear";
            }
        }

        public override List<Point> TargetablePoints(Point characterPosition)
        {
            List<Point> targetablePoints = new List<Point>();

            targetablePoints.Add(characterPosition + new Point(1, 0));
            targetablePoints.Add(characterPosition + new Point(2, 0));
            targetablePoints.Add(characterPosition + new Point(-1, 0));
            targetablePoints.Add(characterPosition + new Point(-2, 0));
            targetablePoints.Add(characterPosition + new Point(0, 1));
            targetablePoints.Add(characterPosition + new Point(0, 2));
            targetablePoints.Add(characterPosition + new Point(0, -1));
            targetablePoints.Add(characterPosition + new Point(0, -2));

            StripImpossibleToTargetPoints(targetablePoints);

            return targetablePoints;
        }
    }
}
