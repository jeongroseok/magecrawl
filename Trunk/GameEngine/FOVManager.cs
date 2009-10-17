using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class FOVManager : IDisposable
    {
        private TCODFov m_fov;

        internal FOVManager(Map map)
        {
            m_fov = new TCODFov(map.Width, map.Height);
            Update(map);
        }

        public void Dispose()
        {
            m_fov.Dispose();
        }

        public void Update(Map map)
        {
            m_fov.ClearMap();
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    bool isWall = map[i, j].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Wall;
                    m_fov.SetCell(i, j, !isWall, !isWall);
                }
            }

            foreach (MapObject obj in map.MapObjects)
            {
                if (obj.IsSolid)
                {
                    m_fov.SetCell(obj.Position.X, obj.Position.Y, false, false);
                }
            }
        }

        // Since we're doing FOV and Pathfinding, there is no good reason to build up visibility/walkability maps twice
        // This function, which should only be called by PathfindingMap, lets us do that.
        internal TCODFov GetTCODFov()
        {
            return m_fov;
        }
    }
}
