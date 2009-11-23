using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Level;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class PathfindingMap : IDisposable
    {
        private TCODPathFinding m_pathFinding;
        private TCODFov m_fov;
        private Map m_map;
        private Player m_player;

        public PathfindingMap(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_fov = new TCODFov(map.Width, map.Height);
            m_fov.ClearMap();
            m_pathFinding = new TCODPathFinding(m_fov, 1.41f);
        }
        
        public void Dispose()
        {
            if (m_pathFinding != null)
                m_pathFinding.Dispose();
            m_pathFinding = null;

            if (m_fov != null)
                m_fov.Dispose();
            m_fov = null;
        }

        // Alright, the behavior we're looking for is a bit unique.
        // We want to know if you can walk to a given position.
        // If there is a character there, ignore it so we can walk 'towards' it
        // If there are doors in the way, if we can operate, ignore them.
        public List<Point> Travel(Character actor, Point dest, bool canOperate, PhysicsEngine engine)
        {
            UpdateInternalFOVForNewRequest(dest, canOperate, engine);

            bool pathExists = m_pathFinding.ComputePath(actor.Position.X, actor.Position.Y, dest.X, dest.Y);
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

        private void UpdateInternalFOVForNewRequest(Point dest, bool canOperate, PhysicsEngine engine)
        {
            // First get the 'default' values
            bool[,] moveableGrid = PhysicsEngine.CalculateMoveablePointGrid(m_map, m_player.Position);

            // Now foor doors, if we can operate, make it movable
            // Doors are special. If they ever are not special, make this a virtual method
            if (canOperate)
            {
                foreach (MapObject m in m_map.MapObjects.OfType<MapDoor>())
                    moveableGrid[m.Position.X, m.Position.Y] = true;
            }

            // Now for a targetted monster if any, they are movable
            Monster monster = (Monster)m_map.Monsters.SingleOrDefault(x => x.Position == dest);
            if (monster != null)
                moveableGrid[monster.Position.X, monster.Position.Y] = true;

            // Same for player
            if (m_player.Position == dest)
                moveableGrid[m_player.Position.X, m_player.Position.Y] = true;

            // Now use moveableGrid to setup FOV
            for (int i = 0; i < m_map.Width; ++i)
            {
                for (int j = 0; j < m_map.Height; ++j)
                {
                    bool isMoveable = moveableGrid[i, j];
                    m_fov.SetCell(i, j, isMoveable, isMoveable);
                }
            }
        }
    }
}
