using System;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public enum MapObjectType
    {
        OpenDoor,
        ClosedDoor,
        TreasureChest,
        Cosmetic,
        StairsUp,
        StairsDown
    }

    public interface IMapObject
    {
        string Name
        {
            get;
        }

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
