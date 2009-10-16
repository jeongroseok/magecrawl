using System;
using System.Collections.Generic;

namespace GameEngine
{
    internal sealed class MapTile : Interfaces.IMapTile
    {
        private Interfaces.TerrainType m_type;
        internal MapTile(Interfaces.TerrainType type)
        {
            m_type = type;
        }

        public GameEngine.Interfaces.TerrainType Terrain
        {
            get 
            {
                return m_type;
            }
        }
    }
}