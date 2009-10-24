using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

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

        public abstract List<WeaponPoint> TargetablePoints(Point characterPosition);

        public float EffectiveStrengthAtPoint(Point characterCenter, Point pointOfInterest)
        {
            foreach (WeaponPoint p in TargetablePoints(characterCenter))
            {
                if (p.Position == pointOfInterest)
                    return p.EffectiveStrength;
            }
            throw new System.ArgumentException("Asked for effective strength at point not targetable?");
        }

        public void StripImpossibleToTargetPoints(List<WeaponPoint> pointList)
        {
            List<WeaponPoint> pointsToRemove = new List<WeaponPoint>();
            bool[,] movabilityTable = CoreGameEngine.Instance.PlayerMoveableToEveryPoint();
            foreach (WeaponPoint p in pointList)
            {
                // We want to be able to target only points that make sense
                bool isGoodPoint = false;
                if (CoreGameEngine.Instance.Map.IsPointOnMap(p.Position))
                {
                    // If we can move there, that's good.
                    isGoodPoint = movabilityTable[p.Position.X, p.Position.Y];

                    // If not, if there's a monster there, we're also good
                    if (!isGoodPoint)
                    {
                        foreach (Monster m in CoreGameEngine.Instance.Map.Monsters)
                        {
                            if (m.Position == p.Position)
                            {
                                isGoodPoint = true;
                                break;
                            }
                        }
                    }

                    // Or a player
                    if (!isGoodPoint)
                    {
                        if (p.Position == CoreGameEngine.Instance.Player.Position)
                            isGoodPoint = true;
                    }
                }
                if (!isGoodPoint)
                {
                    pointsToRemove.Add(p);
                }
            }
            foreach (WeaponPoint p in pointsToRemove)
            {
                pointList.Remove(p);
            }
        }

        public bool PositionInTargetablePoints(Point characterCenter, Point pointOfInterest)
        {
            List<WeaponPoint> targetablePoints = TargetablePoints(characterCenter);
            foreach (WeaponPoint t in targetablePoints)
            {
                if (t.Position == pointOfInterest)
                    return true;
            }
            return false;
        }
    }
}
