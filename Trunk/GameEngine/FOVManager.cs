using System;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Level;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class FOVManager : IDisposable
    {
        private TCODMap m_fov;

        internal FOVManager(PhysicsEngine physicsEngine, Map map)
        {
            m_fov = new TCODMap(map.Width, map.Height);
        }

        public void Dispose()
        {
            if (m_fov != null)
                m_fov.Dispose();
            m_fov = null;
        }

        // New maps might have new height/width
        public void UpdateNewMap(PhysicsEngine physicsEngine, Map map)
        {
            m_fov.Dispose();
            m_fov = new TCODMap(map.Width, map.Height);
        }

        // This is to be private. Only the FOVManager should update.
        private void UpdateFOV(Map map, Point viewPoint, int viewableDistance)
        {
            // We do +2 just to make sure we don't get any border effects. 
            int viewableRadius = viewableDistance + 2;

            // We're going to only setup an area around the viewPoint as "viewable" if it is, so we don't 
            // have to traverse the entire map. 
            Point upperLeftPointViewRange = map.CoercePointOntoMap(viewPoint - new Point(viewableRadius, viewableRadius));
            Point lowerRightPointViewRange = map.CoercePointOntoMap(viewPoint + new Point(viewableRadius, viewableRadius));
            Point viewRange = lowerRightPointViewRange - upperLeftPointViewRange;

            m_fov.clear();

            for (int i = upperLeftPointViewRange.X; i < upperLeftPointViewRange.X + viewRange.X; ++i)
            {
                for (int j = upperLeftPointViewRange.Y; j < upperLeftPointViewRange.Y + viewRange.Y; ++j)
                {
                    bool isFloor = map.GetTerrainAt(i, j) == TerrainType.Floor;
                    m_fov.setProperties(i, j, isFloor, isFloor);
                }
            }

            foreach (MapObject obj in map.MapObjects.Where(x => x.IsSolid))
            {
                m_fov.setProperties(obj.Position.X, obj.Position.Y, obj.IsTransarent, !obj.IsSolid);
            }

            foreach (Monster m in map.Monsters)
            {
                m_fov.setProperties(m.Position.X, m.Position.Y, false, false);
            }
        }

        private void CalculateCore(Map map, Point viewPoint, int viewableDistance)
        {
            UpdateFOV(map, viewPoint, viewableDistance);
            m_fov.computeFov(viewPoint.X, viewPoint.Y, viewableDistance, true, TCODFOVTypes.ShadowFov);
        }

        // Calculate FOV for multipe Visible calls
        public void CalculateForMultipleCalls(Map map, Point viewPoint, int viewableDistance)
        {
            CalculateCore(map, viewPoint, viewableDistance);
        }

        // CalculateForMultipleCalls needs to be called if you want good data.
        public bool Visible(Point pointWantToView)
        {
            return m_fov.isInFov(pointWantToView.X, pointWantToView.Y);
        }

        // Used when we're only calculating a few points and precalculating is not worth it. Use CalculateForMultipleCalls/Visible 
        // for the cases where we're checking multiplePositions
        public bool VisibleSingleShot(Map map, Point viewPoint, int viewableDistance, Point pointWantToView)
        {
            CalculateCore(map, viewPoint, viewableDistance);
            return m_fov.isInFov(pointWantToView.X, pointWantToView.Y);
        }
    }
}
