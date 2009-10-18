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

        internal FOVManager(PhysicsEngine physicsEngine, Map map)
        {
            m_fov = new TCODFov(map.Width, map.Height);
            Update(physicsEngine, map);
        }

        public void Dispose()
        {
            if (m_fov != null)
                m_fov.Dispose();
            m_fov = null;
        }

        public void Update(PhysicsEngine physicsEngine, Map map)
        {
            m_fov.ClearMap();

            // Reusing code from PhysicsEngine now. If we every have 
            // cells that are see through but not walkable, we'll need
            // two calls here
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (physicsEngine.IsPathablePoint(map, new Point(i, j)))
                        m_fov.SetCell(i, j, true, true);
                    else
                        m_fov.SetCell(i, j, false, false);
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
