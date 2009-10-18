using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;
using libtcodWrapper;

namespace Magecrawl.GameEngine
{
    internal class PhysicsEngine : IDisposable
    {
        private CoreTimingEngine m_timingEngine;
        private FOVManager m_fovManager;
        private TCODRandom m_random;

        // Cared and fed by CoreGameEngine, local copy for convenience
        private Player m_player;
        private Map m_map;

        public PhysicsEngine(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_timingEngine = new CoreTimingEngine();
            m_fovManager = new FOVManager(this, map);
            m_random = new TCODRandom();
        }

        public void Dispose()
        {
            if (m_fovManager != null)
                m_fovManager.Dispose();
            m_fovManager = null;
        }

        internal void GameLoaded(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_fovManager.Update(this, m_map);    // We have a new map, recalc LOS
        }

        internal FOVManager FOVManager
        {
            get
            {
                return m_fovManager;
            }
        }

        // Moveable wants player point to be closed (Monsters don't move into player)
        internal bool IsMovablePoint(Map map, Player player, Point p)
        {
            bool isMoveablePoint = IsPathablePoint(map, p);

            if (player.Position == p)
                isMoveablePoint = false;

            return isMoveablePoint;
        }

        // Pathfinding wants player point to be open (Monster Pathfinding)
        internal bool IsPathablePoint(Map map, Point p)
        {
            bool isPathablePoint = map[p.X, p.Y].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Floor;

            foreach (MapObject obj in map.MapObjects)
            {
                if (obj.Position == p && obj.IsSolid)
                    isPathablePoint = false;
            }

            foreach (Monster m in map.Monsters)
            {
                if (m.Position == p)
                    isPathablePoint = false;
            }

            return isPathablePoint;
        }

        internal bool Move(Character c, Direction direction)
        {
            bool didAnything = false;
            Point newPosition = PointDirectionUtils.ConvertDirectionToDestinationPoint(c.Position, direction);
            if (m_map.IsPointOnMap(newPosition) && IsMovablePoint(m_map, m_player, newPosition))
            {
                c.Position = newPosition;
                m_timingEngine.ActorMadeMove(c);
                m_fovManager.Update(this, m_map);    // Operating can change LOS and such
                didAnything = true;
            }
            return didAnything;
        }

        internal bool Attack(Character attacker, Direction direction)
        {
            bool didAnything = false;
            Point attackTarget = PointDirectionUtils.ConvertDirectionToDestinationPoint(attacker.Position, direction);
            foreach (Monster m in m_map.Monsters)
            {
                if (m.Position == attackTarget)
                {
                    m_map.KillMonster(m);
                    didAnything = true;
                    PublicGameEngine.SendTextOutput("Monster Killed.");
                    break;
                }
            }
            if (!didAnything && attackTarget == m_player.Position)
            {
                // TODO: Handle player attack here.
                didAnything = true;
            }
            if (didAnything)
                m_fovManager.Update(this, m_map);
            return didAnything;
        }

        public bool Operate(Character characterOperating, Direction direction)
        {
            bool didAnything = false;

            Point newPosition = PointDirectionUtils.ConvertDirectionToDestinationPoint(characterOperating.Position, direction);
            foreach (MapObject obj in m_map.MapObjects)
            {
                OperableMapObject operateObj = obj as OperableMapObject;
                if (operateObj != null && operateObj.Position == newPosition)
                {
                    operateObj.Operate();
                    m_timingEngine.ActorDidAction(characterOperating);
                    m_fovManager.Update(this, m_map);    // Operating can change LOS and such
                    didAnything = true;
                }
            }

            return didAnything;
        }

        internal bool Wait(Character c)
        {
            m_timingEngine.ActorDidAction(c);
            return true;
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction(CoreGameEngine engine)
        {
            Character nextCharacter = m_timingEngine.GetNextActor(m_player, m_map);
            if (nextCharacter is Player)
                return;

            Monster monster = nextCharacter as Monster;
            MonsterAction result = monster.Action(engine);

            // We don't need to cost moves, since move itself does so already
            // TODO - Make all actions cost, and remove this
            if (result == MonsterAction.DidAction)
                m_timingEngine.ActorDidAction(monster);
        }
    }
}
