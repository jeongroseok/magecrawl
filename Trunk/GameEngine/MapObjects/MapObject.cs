using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal abstract class MapObject : IMapObject, IXmlSerializable
    {
        public abstract bool IsSolid
        {
            get;
        }

        public abstract bool CanOperate
        {
            get;
        }

        public abstract MapObjectType Type
        {
            get;
        }

        public abstract Point Position
        {
            get;
            internal set;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
}