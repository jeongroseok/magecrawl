using System;
using System.Linq;
using libtcod;
using Magecrawl.Actors;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Maps;
using Magecrawl.Maps.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class FOVManager : IFOVManager, IDisposable
    {
        private TCODMap m_fov;
        private Point m_lastCalculatedViewPoint;
        private int m_lastCalculatedVision;

        internal FOVManager(PhysicsEngine physicsEngine, Map map)
        {
            m_fov = new TCODMap(map.Width, map.Height);
            m_lastCalculatedViewPoint = Point.Invalid;
            m_lastCalculatedVision = -1;
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
        private void UpdateFOV(IMapCore map, Point viewPoint, int viewableDistance)
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

        private void CalculateCore(IMapCore map, Point viewPoint, int viewableDistance)
        {
            UpdateFOV(map, viewPoint, viewableDistance);
            m_fov.computeFov(viewPoint.X, viewPoint.Y, viewableDistance, true, TCODFOVTypes.ShadowFov);
        }

        // Calculate FOV for multipe Visible calls
        public void CalculateForMultipleCalls(Map map, Point viewPoint, int viewableDistance)
        {
            m_lastCalculatedViewPoint = viewPoint;
            m_lastCalculatedVision = viewableDistance;
            CalculateCore(map, viewPoint, viewableDistance);
        }

        // CalculateForMultipleCalls needs to be called if you want good data.
        public bool Visible(Point pointWantToView)
        {
            // If we're significantly father than our viewableDistance, give up early
            if (PointDirectionUtils.NormalDistance(m_lastCalculatedViewPoint, pointWantToView) > m_lastCalculatedVision + 3)  // 3 is arbritray large just to prevent edge effects
                return false;

            return m_fov.isInFov(pointWantToView.X, pointWantToView.Y);
        }

        // Used when we're only calculating a few points and precalculating is not worth it. Use CalculateForMultipleCalls/Visible 
        // for the cases where we're checking multiplePositions
        public bool VisibleSingleShot(IMapCore map, Point viewPoint, int viewableDistance, Point pointWantToView)
        {
            // If we're significantly father than our viewableDistance, give up early
            if (PointDirectionUtils.NormalDistance(viewPoint, pointWantToView) > viewableDistance + 3)  // 3 is arbritray large just to prevent edge effects
                return false;

            CalculateCore(map, viewPoint, viewableDistance);
            return m_fov.isInFov(pointWantToView.X, pointWantToView.Y);
        }
    }
}
