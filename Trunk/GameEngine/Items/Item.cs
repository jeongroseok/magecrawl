using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Items
{
    internal abstract class Item : IItem, IXmlSerializable, ICloneable
    {
        private string m_name;
        private string m_itemDescription;
        private string m_flavorText;

        internal Item(string name, string itemDescription, string flavorText)
        {
            m_name = name;
            m_itemDescription = itemDescription;
            m_flavorText = flavorText;
        }

        public abstract object Clone();

        public abstract List<ItemOptions> PlayerOptions
        {
            get;
        }

        public virtual string DisplayName
        {
            get
            {
                return m_name;
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

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        virtual public void ReadXml(XmlReader reader)
        {
        }

        virtual public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", m_name);
        }
    }
}
