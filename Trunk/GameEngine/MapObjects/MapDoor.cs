using System;
using System.Xml;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class MapDoor : OperableMapObject
    {
        private Point m_position;
        private bool m_opened;

        public MapDoor()
            : this(Point.Invalid, false)
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

        public override string Name
        {
            get
            {
                return m_opened ? "Opened Door" : "Closed Door";
            }
        }

        public override MapObjectType Type
        {
            get
            {
                return m_opened ? MapObjectType.OpenDoor : MapObjectType.ClosedDoor;
            }
        }

        public override Point Position
        {
            get
            {
                return m_position;
            }
            internal set
            {
                m_position = value;
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return !m_opened;
            }
        }

        public override bool IsTransarent
        {
            get
            {
                return m_opened;
            }
        }

        public override bool CanOperate
        {
            get 
            {
                return true;
            }
        }

        public override void Operate(ICharacter actor)
        {
            m_opened = !m_opened;
        }

        #region SaveLoad

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

        #endregion
    }
}
