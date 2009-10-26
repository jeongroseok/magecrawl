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
    internal class SimpleShortBow : SimpleBow, Item
    {
        internal SimpleShortBow() : base(null, "Simple Short Bow", new DiceRoll(1, 2, 1))
        {
        }

        public string ItemDescription
        {
            get 
            {
                return "A simple, albeit primitive short bow. Made of a flexable wooden shaft and a sinew bowstring.";
            }
        }

        public string FlavorDescription
        {
            get
            {
                return "Bows like this were taking down small game centuries ago. Simple to make, maintain, and carry.";
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
            writer.WriteElementString("Type", "Simple Bow");
        }
    }
}
