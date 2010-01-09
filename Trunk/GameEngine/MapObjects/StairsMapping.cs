using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal class StairsMapping : IXmlSerializable
    {
        private static StairsMapping m_instance;
        public static StairsMapping Instance
        {
            get
            {
                return m_instance;
            }
        }

        public static void Setup()
        {
            m_instance = new StairsMapping();
        }

        private StairsMapping()
        {
            m_stairMapping = new Dictionary<Guid, Point>();
        }

        private Dictionary<Guid, Point> m_stairMapping;

        public Point GetMapping(Guid g)
        {
            return m_stairMapping[g];
        }

        public void SetMapping(Guid g, Point destination)
        {
            m_stairMapping[g] = destination;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            m_stairMapping = new Dictionary<Guid, Point>();
            reader.ReadStartElement();
            int count = reader.ReadElementContentAsInt();
            for (int i = 0; i < count; ++i)
            {
                Guid g = new Guid(reader.ReadElementContentAsString());
                Point destination = new Point();
                destination = destination.ReadXml(reader);
                m_stairMapping[g] = destination;
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("StairsMappings");
            writer.WriteElementString("Count", m_stairMapping.Keys.Count.ToString());
            foreach (Guid g in m_stairMapping.Keys)
            {
                writer.WriteElementString("StairGUID", g.ToString());
                m_stairMapping[g].WriteToXml(writer, "Destination");
            }
            writer.WriteEndElement();
        }
    }
}
