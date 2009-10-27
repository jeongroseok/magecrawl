using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    // This class should go away when we have an xml reader to create items on the fly. Until then...
    internal class Spear : WeaponBase, Item
    {
        public Spear(string name, DiceRoll damage, string description, string flavorText)
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

            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(1, 0), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(2, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-1, 0), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(-2, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, 1), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, 2), 1.0f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, -1), .5f));
            targetablePoints.Add(new EffectivePoint(m_owner.Position + new Point(0, -2), 1.0f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision);

            return targetablePoints;
        }
    }
}
