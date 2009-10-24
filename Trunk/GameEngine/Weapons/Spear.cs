using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
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

        public override List<WeaponPoint> CalculateTargetablePoints(Point characterCenter)
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(1, 0), .5f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(2, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(-1, 0), .5f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(-2, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(0, 1), .5f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(0, 2), 1.0f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(0, -1), .5f));
            targetablePoints.Add(new WeaponPoint(characterCenter + new Point(0, -2), 1.0f));
            
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints);

            return targetablePoints;
        }
    }
}
