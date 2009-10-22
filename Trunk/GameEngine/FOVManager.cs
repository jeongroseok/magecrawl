using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class FOVManager : IDisposable
    {
        private TCODFov m_fov;

        internal FOVManager(PhysicsEngine physicsEngine, Map map, Player player)
        {
            m_fov = new TCODFov(map.Width, map.Height);
            Update(physicsEngine, map, player);
        }

        public void Dispose()
        {
            if (m_fov != null)
                m_fov.Dispose();
            m_fov = null;
        }

        // New maps might have new height/width
        public void UpdateNewMap(PhysicsEngine physicsEngine, Map map, Player player)
        {
            m_fov.Dispose();
            m_fov = new TCODFov(map.Width, map.Height);
            Update(physicsEngine, map, player);
        }

        public void Update(PhysicsEngine physicsEngine, Map map, Player player)
        {
            m_fov.ClearMap();

            bool[,] moveableGrid = physicsEngine.CalculateMoveablePointGrid(map, player);

            // If we every have cells that are see through but not walkable, we'll need more here
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    bool isMoveable = moveableGrid[i, j];
                    m_fov.SetCell(i, j, isMoveable, isMoveable);
                }
            }
        }

        public void CalculateForMultipleCalls(Point viewPoint, int viewableDistance)
        {
            m_fov.CalculateFOV(viewPoint.X, viewPoint.Y, viewableDistance, true, FovAlgorithm.Shadow);
        }

        public bool Visible(Point pointWantToView)
        {
            return m_fov.CheckTileFOV(pointWantToView.X, pointWantToView.Y);
        }

        // Used when we're only calculating a few points and precalculating is not worth it. Use CalculateForMultipleCalls/Visible 
        // for the common case.
        public bool VisibleSingleShot(Point viewPoint, int viewableDistance, Point pointWantToView)
        {
            m_fov.CalculateFOV(viewPoint.X, viewPoint.Y, viewableDistance, true, FovAlgorithm.Shadow);
            return m_fov.CheckTileFOV(pointWantToView.X, pointWantToView.Y);
        }
    }
}
