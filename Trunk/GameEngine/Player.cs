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
            throw new NotImplementedException();
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
