using System;
using System.Collections.Generic;

namespace Magecrawl.GameEngine.Interfaces
{
    public enum TerrainType
    { 
        Floor, Wall 
    }

    public interface IMapTile
    {
        TerrainType Terrain
        {
            get;
        }
    }
}
