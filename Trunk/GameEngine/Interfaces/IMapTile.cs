using System;
using System.Collections.Generic;

namespace GameEngine.Interfaces
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
