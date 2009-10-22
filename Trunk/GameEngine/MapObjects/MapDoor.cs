using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class MapDoor : OperableMapObject
    {
        private Point m_position;
        private bool m_opened;

        public MapDoor()
            : this(new Point(-1, -1), false)
        {
        }

        public MapDoor(Point position)
            : this(position, false)
        {
        }

        public MapDoor(Point position, bool isOpen)
        {
            m_position = position;
            m_opened = isOpen;
        }

        public override MapObjectType Type
        {
            get
            {
                if (m_opened)
                    return MapObjectType.OpenDoor;
                else
                    return MapObjectType.ClosedDoor;
            }
        }

        public override Point Position
        {
            get
            {
                return m_position;
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return !m_opened;
            }
        }

        public override void Operate()
        {
            m_opened = !m_opened;
        }

        public override void ReadXml(XmlReader reader)
        {
            m_opened = Boolean.Parse(reader.ReadElementContentAsString());
            m_position = m_position.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", "MapDoor");
            writer.WriteElementString("DoorOpen", m_opened.ToString());
            m_position.WriteToXml(writer, "Position");
        }
    }
}
