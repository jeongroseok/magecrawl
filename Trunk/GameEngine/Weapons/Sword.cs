using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class Sword : WeaponBase
    {
        internal Sword(ICharacter owner)
        {
            m_owner = owner;
        }

        public override DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(1, 8);
            }
        }

        public override string Name
        {
            get 
            {
                return "Sword";
            }
        }

        public override List<WeaponPoint> CalculateTargetablePoints()
        {
            List<WeaponPoint> targetablePoints = new List<WeaponPoint>();

            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, 1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(0, -1), 1.0f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, 1), .75f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, -1), .75f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(-1, 1), .75f));
            targetablePoints.Add(new WeaponPoint(m_owner.Position + new Point(1, -1), .75f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision);

            return targetablePoints;
        }
    }
}
