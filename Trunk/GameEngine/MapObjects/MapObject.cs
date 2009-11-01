using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;

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

        public abstract Magecrawl.Utilities.Point Position
        {
            get;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);

        internal static MapObject CreateMapObjectFromTypeString(string s)
        {
            switch (s)
            {
                case "MapDoor":
                    return new MapDoor();
                case "TreasureChest":
                    return new TreasureChest();
                default:
                    throw new System.ArgumentException("Invalid type in CreateMapObjectFromTypeString");
            }
        }
    }
}