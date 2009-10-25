using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons.BaseTypes
{
    internal abstract class WeaponBase : IWeapon
    {
        protected ICharacter m_owner;
        protected string m_name;
        protected DiceRoll m_damage;

        internal ICharacter Owner
        {
            get
            {
                return m_owner;
            }
            set
            {
                m_owner = value;
            }
        }

        public virtual DiceRoll Damage
        {
            get
            {
                return m_damage;
            }
            internal set
            {
                m_damage = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return m_name;
            }
            internal set
            {
                m_name = value;
            }
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
