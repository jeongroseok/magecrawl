using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Potion : Item
    {
        private string m_name;
        private string m_itemDescription;
        private string m_flavorText;
        private string m_effectType;
        private int m_strength;

        internal Potion(string name, string effectType, int strength, string itemDescription, string flavorText)
        {
            m_name = name;
            m_effectType = effectType;
            m_strength = strength;
            m_itemDescription = itemDescription;
            m_flavorText = flavorText;            
        }

        public string Name
        {
            get 
            {
                return m_name;
            }
        }

        public string DisplayName
        {
            get
            {
                return Name;
            }
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

        public string EffectType
        {
            get
            {
                return m_effectType;
            }
        }

        public int Strength
        {
            get
            {
                return m_strength;
            }
        }

        public List<Magecrawl.GameEngine.Interfaces.ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Drink", true),
                    new ItemOptions("Drop", true)
                };
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", m_name);
        }
    }
}
