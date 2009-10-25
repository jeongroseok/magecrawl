using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Magecrawl.GameEngine.SaveLoad
{
    public delegate void ReadListFromXMLCore(XmlReader reader);

    internal static class ListSerialization
    {
        public static void WriteListToXML<T>(XmlWriter writer, List<T> list, string listName) where T : IXmlSerializable
        {
            writer.WriteStartElement(listName + "List");
            writer.WriteElementString("Count", list.Count.ToString());
            for (int i = 0; i < list.Count; ++i)
            {
                IXmlSerializable current = list[i];
                writer.WriteStartElement(string.Format(listName + "{0}", i));
                current.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public static void ReadListFromXML(XmlReader reader, ReadListFromXMLCore readDelegate)
        {
            reader.ReadStartElement();

            int mapFeatureListLength = reader.ReadElementContentAsInt();

            for (int i = 0; i < mapFeatureListLength; ++i)
            {
                reader.ReadStartElement();

                readDelegate(reader);

                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }
    }
}
