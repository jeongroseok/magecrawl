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
    internal class SimpleBow : WeaponBase, Item
    {
        public SimpleBow(string name, DiceRoll damage, string description, string flavorText)
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

            const int SimpleBowRange = 6;
            for (int i = -SimpleBowRange; i <= SimpleBowRange; ++i)
            {
                for (int j = -SimpleBowRange; j <= SimpleBowRange; ++j)
                {
                    int distance = System.Math.Abs(i) + Math.Abs(j);
                    bool allowable = (distance <= SimpleBowRange) && (distance > 2);
                    float weaponStrength = 1.0f - (Math.Max(distance - 4, 0) * .25f);
                    if (allowable)
                        targetablePoints.Add(new EffectivePoint(new Point(m_owner.Position.X + i, m_owner.Position.Y + j), weaponStrength));
                }
            }

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision);

            return targetablePoints;
        }
    }
}
