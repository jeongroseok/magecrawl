﻿using System;
using System.Collections.Generic;

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

        IEnumerable<IMapObject> MapObjects
        {
            get;
        }

        IEnumerable<ICharacter> Monsters
        {
            get;
        }

        IMapTile this[int width, int height]
        {
            get;
        }
    }
}