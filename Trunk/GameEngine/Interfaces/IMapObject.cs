using System;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public enum MapObjectType
    {
        OpenDoor,
        ClosedDoor,
        TreasureChest,
        Cosmetic
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

        bool IsSolid
        {
            get;
        }

        bool CanOperate
        {
            get;
        }
    }
}
