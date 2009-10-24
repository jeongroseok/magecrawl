using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class SimpleBow : IWeapon
    {   
        internal SimpleBow()
        {
        }

        public DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(1, 1);
            }
        }

        public string Name
        {
            get 
            {
                return "Bow";
            }
        }

        public List<Point> TargetablePoints(Point characterPosition)
        {
            List<Point> targetablePoints = new List<Point>();

            const int SimpleBowRange = 4;
            for (int i = -SimpleBowRange; i <= SimpleBowRange; ++i)
            {
                for (int j = -SimpleBowRange; j <= SimpleBowRange; ++j)
                {
                    bool allowable = (System.Math.Abs(i) + System.Math.Abs(j)) <= SimpleBowRange;
                    if (allowable)
                        targetablePoints.Add(new Point(characterPosition.X + i, characterPosition.Y + j));
                }
            }
            
            return targetablePoints;
        }
    }
}
