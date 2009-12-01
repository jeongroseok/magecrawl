using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.SaveLoad;

namespace Magecrawl.GameEngine.MapObjects
{
    internal class Stairs : MapObject
    {
        private Point m_position;
        private MapObjectType m_type;

        internal Stairs(Point position, bool stairsUp)
        {
            m_type = stairsUp ? MapObjectType.StairsUp : MapObjectType.StairsDown;
            m_position = position;
        }

        public override bool IsSolid
        {
            get 
            {
                return false;
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
            internal set
            {
                m_position = value;
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", m_type == MapObjectType.StairsUp ? "Stairs Up" : "Stairs Down");
            m_position.WriteToXml(writer, "Position");
        }
    }
}
