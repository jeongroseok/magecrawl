using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    public sealed class Player : Interfaces.IPlayer, IXmlSerializable
    {
        private Point m_position;

        public Player()
        {
            m_position = new Point(-1, -1);
        }

        public Player(int x, int y)
        {
            m_position = new Point(x, y);
        }

        public Point Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public string Name
        {
            get
            {
                return "Donblas";
            }
        }

        #region SaveLoad

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            reader.ReadElementString(); // Ignore name for now
            m_position = m_position.ReadXml(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            writer.WriteElementString("Name", Name);
            Position.WriteToXml(writer, "Position");
        }

        #endregion
    }
}
