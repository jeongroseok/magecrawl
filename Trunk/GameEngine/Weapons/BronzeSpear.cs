using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Weapons.BaseTypes;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    // This class should go away when we have an xml reader to create items on the fly. Until then...
    internal class BronzeSpear : Spear, Item
    {
        internal BronzeSpear() : base(null, "Bronze Spear", new DiceRoll(2, 2))
        {
        }

        public string ItemDescription
        {
            get 
            {
                return "A simple spear, topped with a bronze tip.";
            }
        }

        public string FlavorDescription
        {
            get
            {
                return "The spear has been used as a thrusting weapon for ages, providing reach to strike at the enemy first.";
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
            writer.WriteElementString("Type", "Bronze Spear");
        }
    }
}
