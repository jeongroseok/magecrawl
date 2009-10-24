using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Weapons
{
    internal abstract class WeaponBase : IWeapon
    {
        public abstract DiceRoll Damage
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract List<Point> TargetablePoints(Point characterPosition);

        public void StripImpossibleToTargetPoints(List<Point> pointList)
        {
            List<Point> pointsToRemove = new List<Point>();
            bool[,] movabilityTable = CoreGameEngine.Instance.PlayerMoveableToEveryPoint();
            foreach (Point p in pointList)
            {
                bool isGoodPoint = false;
                // We want to be able to target only points that make sense
                if (CoreGameEngine.Instance.Map.IsPointOnMap(p))
                {
                    // If we can move there, that's good.
                    isGoodPoint = movabilityTable[p.X, p.Y];

                    // If not, if there's a monster there, we're also good
                    if (!isGoodPoint)
                    {
                        foreach (Monster m in CoreGameEngine.Instance.Map.Monsters)
                        {
                            if (m.Position == p)
                            {
                                isGoodPoint = true;
                                break;
                            }
                        }
                    }

                    // Or a player
                    if (!isGoodPoint)
                    {
                        if (p == CoreGameEngine.Instance.Player.Position)
                            isGoodPoint = true;
                    }
                }
                if (!isGoodPoint)
                {
                    pointsToRemove.Add(p);
                }
            }
            foreach (Point p in pointsToRemove)
            {
                pointList.Remove(p);
            }
        }
    }
}
