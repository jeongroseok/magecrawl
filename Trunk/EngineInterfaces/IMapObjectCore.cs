using System.Xml.Serialization;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public interface IMapObjectCore : IMapObject, IXmlSerializable
    {
        new Point Position
        {
            get;
            set;
        }
    }
}
