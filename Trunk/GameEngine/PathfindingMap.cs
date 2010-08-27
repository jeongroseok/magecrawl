using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.Actors;
using Magecrawl.Maps;
using Magecrawl.Maps.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class PathfindingMap : IDisposable
    {
        private TCODPath m_pathFinding;
        private TCODMap m_fov;
        private Map m_map;
        private Player m_player;

        public PathfindingMap(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_fov = new TCODMap(map.Width, map.Height);
            m_fov.clear();
            m_pathFinding = new TCODPath(m_fov, 1.41f);
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
        public List<Point> Travel(Character actor, Point dest, bool canOperate, PhysicsEngine engine, bool usePlayerLOS, bool monstersBlockPath)
        {
            UpdateInternalFOV(actor.Position, dest, canOperate, engine, usePlayerLOS, monstersBlockPath);

            bool pathExists = m_pathFinding.compute(actor.Position.X, actor.Position.Y, dest.X, dest.Y);
            if (!pathExists)
                return null;
            
            List<Point> path = new List<Point>();
            int pathLength = m_pathFinding.size();
            for (int i = 0; i < pathLength; ++i)
            {
                int currentX;
                int currentY;
                m_pathFinding.get(i, out currentX, out currentY);
                path.Add(new Point(currentX, currentY));
            }

            return path;
        }

        private void UpdateInternalFOV(Point source, Point dest, bool canOperate, PhysicsEngine engine, bool usePlayerLOS, bool monstersBlockPath)
        {
            // First get the 'default' values
            bool[,] moveableGrid = m_map.CalculateMoveablePointGrid(monstersBlockPath, source);

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

            if (usePlayerLOS)
            {
                for (int i = 0; i < m_map.Width; ++i)
                {
                    for (int j = 0; j < m_map.Height; ++j)
                    {
                        if (!m_map.IsVisitedAt(new Point(i, j)))
                        {
                            moveableGrid[i, j] = false;
                        }
                    }
                }
            }

            // Now use moveableGrid to setup FOV
            for (int i = 0; i < m_map.Width; ++i)
            {
                for (int j = 0; j < m_map.Height; ++j)
                {
                    bool isMoveable = moveableGrid[i, j];
                    m_fov.setProperties(i, j, isMoveable, isMoveable);
                }
            }
        }
    }
}
