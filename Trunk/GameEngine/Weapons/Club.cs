using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class Club : WeaponBase, Item
    {
        public Club(string name, DiceRoll damage, string description, string flavorText)
        {
            m_itemDescription = description;
            m_flavorText = flavorText;
            m_owner = null;
            m_name = name;
            m_damage = damage;
        }

        public override List<EffectivePoint> CalculateTargetablePoints()
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, 1), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, -1), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, 1), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, -1), .5f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision, true);

            return targetablePoints;
        }
    }
}
