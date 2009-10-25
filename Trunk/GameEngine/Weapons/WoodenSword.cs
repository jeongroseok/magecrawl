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
    internal class WoodenSword : Sword, Item
    {
        internal WoodenSword() : base(null, "Wooden Sword", new DiceRoll(1, 4))
        {
        }

        public string ItemDescription
        {
            get 
            {
                return "A simple wooden sword. Full of spliters.";
            }
        }

        public string FlavorDescription
        {
            get
            {
                return "Swords like these are used by youth to build strong muscles.";
            }
        }

        public List<ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Drop", true),
                    new ItemOptions("Equip", true) 
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
            writer.WriteElementString("Type", "Wooden Sword");
        }
    }
}
