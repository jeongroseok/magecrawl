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
            m_flavorText = String.Empty;
            m_name = "Melee";
        }

        public override DiceRoll Damage
        {
            get 
            {
                return m_owner.MeleeDamage;
            }
        }

        public override double CTCostToAttack
        {
            get
            {
                return m_owner.MeleeSpeed;
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

        public override List<EffectivePoint> CalculateTargetablePoints()
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, -1), 1.0f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision, true);

            return targetablePoints;
        }
    }
}
