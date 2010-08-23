using System.Xml;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Maps.MapObjects
{
    public abstract class MapObject : IMapObjectCore
    {
        public abstract string Name
        {
            get;
        }

        public abstract bool IsSolid
        {
            get;
        }

        public abstract bool IsTransarent
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
            set;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
}