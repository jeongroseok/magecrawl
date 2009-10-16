using System;
using System.Collections.Generic;

namespace GameEngine.Interfaces
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

        IEnumerable<IMapObject> MapObjects
        {
            get;
        }

        IMapTile this[int width, int height]
        {
            get;
        }
    }
}
