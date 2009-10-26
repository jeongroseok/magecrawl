using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IMap
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        IList<IMapObject> MapObjects
        {
            get;
        }

        IList<ICharacter> Monsters
        {
            get;
        }

        IList<Pair<IItem, Point>> Items
        {
            get;
        }

        IMapTile this[int width, int height]
        {
            get;
        }
    }
}
