using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.MapObjects
{
    internal abstract class MapObject : IMapObject
    {
        public abstract bool IsSolid
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
    }
}