using System;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public enum MapObjectType
    {
        OpenDoor,
        ClosedDoor,
        TreasureChest
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
