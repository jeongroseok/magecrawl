using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.Interfaces;

namespace Magecrawl.Items
{
    public abstract class Item : IItem, IXmlSerializable
    {
        internal Item()
        {
            Attributes = new Dictionary<string, string>();
        }

        public abstract string DisplayName { get; }
        public abstract string ItemDescription { get; }
        public abstract string FlavorDescription { get; }

        public Dictionary<string, string> Attributes;

        virtual public string ItemEffectSchool
        {
            get
            {
                return null;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        abstract public void ReadXml(XmlReader reader);
        abstract public void WriteXml(XmlWriter writer);

        static public void WriteXmlEntireNode(Item item, string elementName, XmlWriter writer)
        {
            writer.WriteStartElement(elementName);
            if (item != null)
                item.WriteXml(writer);
            else
                writer.WriteElementString("Type", "None");
            writer.WriteEndElement();
        }

        static public Item ReadXmlEntireNode(XmlReader reader)
        {
            Item returnItem = null;
            reader.ReadStartElement();
            string equipedWeaponTypeString = reader.ReadElementString();
            if (equipedWeaponTypeString != "None")
            {
                returnItem = ItemFactory.Instance.CreateBaseItem(equipedWeaponTypeString);
                returnItem.ReadXml(reader);
            }
            reader.ReadEndElement();
            return returnItem;
        }
    }
}
