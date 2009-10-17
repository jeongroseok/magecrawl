using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class PathfindingMap : IDisposable
    {
        private TCODPathFinding m_pathFinding;
        
        public PathfindingMap(FOVManager fovManager)
        {
            m_pathFinding = new TCODPathFinding(fovManager.GetTCODFov(), 1.41f);
        }
        
        public void Dispose()
        {
            m_pathFinding.Dispose();
        }

        public List<Point> Travel(Point initial, Point final)
        {
            bool pathExists = m_pathFinding.ComputePath(initial.X, initial.Y, final.X, final.Y);
            if (!pathExists)
                return null;
            
            List<Point> path = new List<Point>();
            int pathLength = m_pathFinding.GetPathSize();
            for (int i = 0; i < pathLength; ++i)
            {
                int currentX;
                int currentY;
                m_pathFinding.GetPointOnPath(i, out currentX, out currentY);
                path.Add(new Point(currentX, currentY));
            }

            return path;
        }
    }
}
