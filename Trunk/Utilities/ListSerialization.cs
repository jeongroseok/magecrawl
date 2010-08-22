using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Magecrawl.Utilities
{
    public delegate void ReadListFromXMLCore(XmlReader reader);

    public static class ListSerialization
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

        public static void WriteListToXML<T>(XmlWriter writer, List<Pair<T, Point>> list, string listName) where T : IXmlSerializable
        {
            writer.WriteStartElement(listName + "List");
            writer.WriteElementString("Count", list.Count.ToString());
            for (int i = 0; i < list.Count; ++i)
            {
                IXmlSerializable current = list[i].First;
                writer.WriteStartElement(string.Format(listName + "{0}", i));
                current.WriteXml(writer);
                list[i].Second.WriteToXml(writer, "Position");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public static void ReadListFromXML(XmlReader reader, ReadListFromXMLCore readDelegate)
        {
            reader.ReadStartElement();

            int listLength = reader.ReadElementContentAsInt();

            for (int i = 0; i < listLength; ++i)
            {
                reader.ReadStartElement();

                readDelegate(reader);

                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }
    }
}
