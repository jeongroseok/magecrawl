using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal abstract class WeaponBase : IWeapon
    {
        protected ICharacter m_owner;

        public abstract DiceRoll Damage
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract List<WeaponPoint> CalculateTargetablePoints();

        public float EffectiveStrengthAtPoint(Point pointOfInterest)
        {
            foreach (WeaponPoint p in CalculateTargetablePoints())
            {
                if (p.Position == pointOfInterest)
                    return p.EffectiveStrength;
            }
            throw new System.ArgumentException("Asked for effective strength at point not targetable?");
        }

        public bool PositionInTargetablePoints(Point pointOfInterest)
        {
            return PositionInTargetablePoints(pointOfInterest, CalculateTargetablePoints());
        }

        public bool PositionInTargetablePoints(Point pointOfInterest, List<WeaponPoint> targetablePoints)
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
