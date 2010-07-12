using System.Xml;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal class Cosmetic : MapObject
    {
        private Point m_position;

        public Cosmetic() : this(Point.Invalid)
        {
        }

        public Cosmetic(Point position)
        {
            m_position = position;
        }

        public override string Name
        {
            get
            {
                return "Table";
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return true;
            }
        }

        public override bool IsTransarent
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
                return MapObjectType.Cosmetic;
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

        public override void ReadXml(XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", "Cosmetic");
            m_position.WriteToXml(writer, "Position");
        }
    }
}
