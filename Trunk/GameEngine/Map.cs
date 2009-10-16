using System;
using System.Collections.Generic;
using GameEngine.Interfaces;

namespace GameEngine
{
    internal sealed class Map : Interfaces.IMap
    {
        private int m_width;
        private int m_height;
        private MapTile[,] m_map;

        internal Map(int width, int height)
        {
            m_width = width;
            m_height = height;
            m_map = new MapTile[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    TerrainType type = TerrainType.Floor;
                    if (i == 0 || j == 0 || i == (m_width - 1) || j == (m_height - 1))
                        type = TerrainType.Wall;

                    m_map[i, j] = new MapTile(type);
                }
            }
        }

        public int Width
        {
            get
            {
                return m_width;
            }
        }

        public int Height
        {
            get
            {
                return m_height;
            }
        }

        public IMapTile this[int width, int height]
        {
            get 
            {
                return m_map[width, height];
            }
        }
    }
}
