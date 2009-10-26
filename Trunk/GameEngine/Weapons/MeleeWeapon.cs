using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class MeleeWeapon : WeaponBase
    {
        internal MeleeWeapon(ICharacter owner)
        {
            m_owner = owner;
            m_itemDescription = "Your Natural Weapons";
            m_flavorText = "";
            m_name = "Melee";
            m_damage = m_owner.MeleeDamage;
        }

        public override DiceRoll Damage
        {
            get 
            {
                return m_owner.MeleeDamage;
            }
        }

        public override string Name
        {
            get 
            {
                return "Melee";
            }
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Equip", true),
                };
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
