using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class SimpleBow : WeaponBase
    {   
        internal SimpleBow()
        {
        }

        public override DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(1, 2, 1);
            }
        }

        public override string Name
        {
            get 
            {
                return "Bow";
            }
        }

        public override List<Point> TargetablePoints(Point characterPosition)
        {
            List<Point> targetablePoints = new List<Point>();

            const int SimpleBowRange = 5;
            for (int i = -SimpleBowRange; i <= SimpleBowRange; ++i)
            {
                for (int j = -SimpleBowRange; j <= SimpleBowRange; ++j)
                {
                    int distance = (System.Math.Abs(i) + System.Math.Abs(j));
                    bool allowable = (distance <= SimpleBowRange) && (distance > 2);
                    if (allowable)
                        targetablePoints.Add(new Point(characterPosition.X + i, characterPosition.Y + j));
                }
            }

            StripImpossibleToTargetPoints(targetablePoints);
            
            return targetablePoints;
        }
    }
}
