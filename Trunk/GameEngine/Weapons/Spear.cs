﻿using System;
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
        private string m_itemDescription;
        private string m_flavorText;

        internal Spear(string name, DiceRoll damage, string description, string flavorText)
        {
            m_itemDescription = description;
            m_flavorText = flavorText;
            m_owner = null;
            m_name = name;
            m_damage = damage;
        }

        public string ItemDescription
        {
            get 
            {
                return m_itemDescription;
            }
        }

        public string FlavorDescription
        {
            get
            {
                return m_flavorText;
            }
        }

        public List<ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Equip", true),
                    new ItemOptions("Drop", true)
                };
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", m_name);
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
