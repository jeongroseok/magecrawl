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

        internal Dictionary<string, string> Attributes;

        public virtual bool ContainsAttribute(string key)
        {
            return Attributes.ContainsKey(key);
        }

        public virtual string GetAttribute(string key)
        {
            return Attributes[key];
        }

        public void SetExistentAttribute(string key, string value)
        {
            if (!Attributes.ContainsKey(key))
                throw new System.InvalidOperationException("Can't SetExistentAttribute on an attribute that isn't set on the root Attributes");
            Attributes[key] = value;
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
            string equipedWeaponTypeString = GetTypeString(reader);
            if (equipedWeaponTypeString != "None")
            {
                returnItem = ItemFactory.Instance.CreateBaseItem(equipedWeaponTypeString);
                returnItem.ReadXml(reader);
            }
            reader.ReadEndElement();
            return returnItem;
        }

        private static string GetTypeString(XmlReader reader)
        {
#if SILVERLIGHT
            // Silverlight is missing ReadElementString
            reader.Read();
            string equipedWeaponTypeString = reader.Value;
            reader.Read();
            reader.Read();
#else
            string equipedWeaponTypeString = reader.ReadElementString();
#endif
            return equipedWeaponTypeString;
        }
    }
}
