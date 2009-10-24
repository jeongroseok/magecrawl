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

        public abstract List<WeaponPoint> CalculateTargetablePoints(Point characterCenter);

        public float EffectiveStrengthAtPoint(Point characterCenter, Point pointOfInterest)
        {
            foreach (WeaponPoint p in CalculateTargetablePoints(characterCenter))
            {
                if (p.Position == pointOfInterest)
                    return p.EffectiveStrength;
            }
            throw new System.ArgumentException("Asked for effective strength at point not targetable?");
        }

        public bool PositionInTargetablePoints(Point characterCenter, Point pointOfInterest)
        {
            return PositionInTargetablePoints(characterCenter, pointOfInterest, CalculateTargetablePoints(characterCenter));
        }

        public bool PositionInTargetablePoints(Point characterCenter, Point pointOfInterest, List<WeaponPoint> targetablePoints)
        {
            foreach (WeaponPoint t in targetablePoints)
            {
                if (t.Position == pointOfInterest)
                    return true;
            }
            return false;
        }
    }
}
