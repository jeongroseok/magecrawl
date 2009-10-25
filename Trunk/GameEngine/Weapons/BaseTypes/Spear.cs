using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons.BaseTypes
{
    internal abstract class Spear : WeaponBase
    {
        internal Spear()
        {
            m_owner = null;
            m_name = null;
            m_damage = DiceRoll.Invalid;
        }

        internal Spear(ICharacter owner, string name, DiceRoll damamge)
        {
            m_owner = owner;
            m_name = name;
            m_damage = Damage;
        }

        public override List<WeaponPoint> CalculateTargetablePoints()
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, 0), .5f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(2, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, 0), .5f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-2, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, 1), .5f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, 2), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, -1), .5f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, -2), 1.0f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision);

            return targetablePoints;
        }
    }
}
