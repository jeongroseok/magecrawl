using GameEngine.Interfaces;

namespace GameEngine.MapObjects
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

        public abstract Utilities.Point Position
        {
            get;
        }
    }
}