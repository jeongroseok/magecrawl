using System;
using System.Collections.Generic;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class MapObjectFactory
    {
        internal MapObject CreateMapObject(string name)
        {
            switch (name)
            {
                case "MapDoor":
                    return new MapDoor();
                case "TreasureChest":
                    return new TreasureChest();
                default:
                    throw new System.ArgumentException("Invalid type in MapObjectFactory:CreateMapObject");
            }
        }
    }
}
