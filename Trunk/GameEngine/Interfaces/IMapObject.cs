using System;
using Utilities;

namespace GameEngine.Interfaces
{
    public enum MapObjectType
    {
        OpenDoor,
        ClosedDoor
    }

    public interface IMapObject
    {
        MapObjectType Type
        {
            get;
        }

        Point Position
        {
            get;
        }
    }
}
