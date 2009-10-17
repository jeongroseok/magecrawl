using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class Player : Character, Interfaces.IPlayer, IXmlSerializable
    {
        public Player()
        {
            m_position = new Point(-1, -1);
            m_CT = 0;
        }

        public Player(int x, int y)
        {
            m_position = new Point(x, y);
        }

        public string Name
        {
            get
            {
                return "Donblas";
            }
        }

        #region SaveLoad

        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            reader.ReadElementString(); // Ignore name for now
            m_position = m_position.ReadXml(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            writer.WriteElementString("Name", Name);
            Position.WriteToXml(writer, "Position");
        }

        #endregion
    }
}
