using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine
{
    internal sealed class MapTile : IMapTile
    {
        private TerrainType m_type;

        internal MapTile()
        {
            m_type = TerrainType.Wall;
        }

        internal MapTile(Interfaces.TerrainType type)
        {
            m_type = type;
        }

        public TerrainType Terrain
        {
            get 
            {
                return m_type;
            }
        }

        internal char ConvertToChar()
        {
            switch (m_type)
            {
                case TerrainType.Floor:
                    return '.';
                case TerrainType.Wall:
                    return '#';
                default:
                    throw new System.ArgumentException("Invalid Character - ConvertToChar");
            }
        }

        internal void CovertFromChar(char c)
        {
            switch (c)
            {
                case '.':
                    m_type = TerrainType.Floor;
                    break;
                case '#':
                    m_type = TerrainType.Wall;
                    break;
                default:
                    throw new System.ArgumentException("Invalid Character - CovertFromChar");
            }
        }
    }
}