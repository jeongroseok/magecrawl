using System;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Maps.MapObjects
{
    public class StairsUp : Stairs
    {
        public StairsUp(Point position) : base(position, true)
        {
        }
    }

    public class StairsDown : Stairs
    {
        public StairsDown(Point position) : base(position, false)
        {
        }
    }

    public class Stairs : MapObject
    {
        private Point m_position;
        private MapObjectType m_type;
        private Guid m_guid;

        public Stairs(Point position, bool stairsUp)
        {
            m_type = stairsUp ? MapObjectType.StairsUp : MapObjectType.StairsDown;
            m_position = position;
            m_guid = Guid.NewGuid();
        }

        public override string Name
        {
            get 
            {
                return m_type == MapObjectType.StairsUp ? "Stairs Up" : "Stairs Down";
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return false;
            }
        }

        public override bool IsTransarent
        {
            get
            {
                return true;
            }
        }

        public override bool CanOperate
        {
            get 
            {
                return false;
            }
        }

        public override MapObjectType Type
        {
            get 
            {
                return m_type;
            }
        }

        public override Point Position
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

        public Guid UniqueID
        {
            get
            {
                return m_guid;
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
            m_guid = new Guid(reader.ReadElementContentAsString());
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", m_type == MapObjectType.StairsUp ? "StairsUp" : "StairsDown");
            m_position.WriteToXml(writer, "Position");
            writer.WriteElementString("Guid", m_guid.ToString());
        }
    }
}
