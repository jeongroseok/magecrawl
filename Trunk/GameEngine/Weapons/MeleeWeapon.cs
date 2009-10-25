using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Weapons.BaseTypes;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class MeleeWeapon : WeaponBase
    {
        internal MeleeWeapon(ICharacter owner)
        {
            m_owner = owner;
        }

        public override DiceRoll Damage
        {
            get 
            {
                return m_owner.MeleeDamage;
            }
            internal set
            {
                throw new System.ArgumentException("Can't set damage of MeleeWeapon");
            }
        }

        public override string Name
        {
            get 
            {
                return "Melee";
            }
            internal set
            {
                throw new System.ArgumentException("Can't set name of MeleeWeapon");
            }
        }

        public override List<WeaponPoint> CalculateTargetablePoints()
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, -1), 1.0f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision);

            return targetablePoints;
        }
    }
}
