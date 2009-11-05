using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class MapObjectFactory
    {
        internal MapObject CreateMapObject(string name)
        {
            return CreateMapObject(name, Point.Invalid);
        }

        internal MapObject CreateMapObject(string name, Point position)
        {
            switch (name)
            {
                case "Map Door":
                    return new MapDoor(position);
                case "Treasure Chest":
                    return new TreasureChest(position);
                default:
                    throw new System.ArgumentException("Invalid type in MapObjectFactory:CreateMapObject");
            }
        }
    }
}
