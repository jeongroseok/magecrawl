using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class PhysicsEngine : IDisposable
    {
        private CoreTimingEngine m_timingEngine;
        private FOVManager m_fovManager;
        private CombatEngine m_combatEngine;

        // Cared and fed by CoreGameEngine, local copy for convenience
        private Player m_player;
        private Map m_map;

        public PhysicsEngine(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_timingEngine = new CoreTimingEngine();
            m_fovManager = new FOVManager(this, map, player);
            m_combatEngine = new CombatEngine(player, map);
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
            m_combatEngine.GameLoaded(player, map);

            // We have a new map, recalc LOS with a new map
            m_fovManager.UpdateNewMap(this, m_map, player);    
        }

        internal FOVManager FOVManager
        {
            get
            {
                return m_fovManager;
            }
        }

        // There are many times we want to know what cells are movable into, for FOV or Pathfinding for example
        // This, unlike IsMoveablePoint, calculated them in batch much much more quickly. True means you can walk there
        internal bool[,] CalculateMoveablePointGrid(Map map, Player player)
        {
            bool[,] returnValue = new bool[map.Width, map.Height];

            for (int i = 0; i < m_map.Width; ++i)
            {
                for (int j = 0; j < m_map.Height; ++j)
                {
                    returnValue[i, j] = m_map[i, j].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Floor;
                }
            }

            foreach (MapObject obj in m_map.MapObjects)
            {
                if (obj.IsSolid)
                {
                    returnValue[obj.Position.X, obj.Position.Y] = false;
                }
            }

            foreach (Monster m in m_map.Monsters)
            {
                returnValue[m.Position.X, m.Position.Y] = false;
            }

            returnValue[m_player.Position.X, m_player.Position.Y] = false;

            return returnValue;
        }

        // This is a slow operation. It should not be called multiple times in a row!
        // Call CalculateMoveablePointGrid instead~!
        internal bool IsMovablePoint(Map map, Player player, Point p)
        {
            bool isMoveablePoint = map[p.X, p.Y].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Floor;

            foreach (MapObject obj in map.MapObjects)
            {
                if (obj.Position == p && obj.IsSolid)
                {
                    isMoveablePoint = false;
                    break;
                }
            }

            foreach (Monster m in map.Monsters)
            {
                if (m.Position == p)
                {
                    isMoveablePoint = false;
                    break;
                }
            }

            if (player.Position == p)
                isMoveablePoint = false;

            return isMoveablePoint;
        }

        internal bool Move(Character c, Direction direction)
        {
            bool didAnything = false;
            Point newPosition = PointDirectionUtils.ConvertDirectionToDestinationPoint(c.Position, direction);
            if (m_map.IsPointOnMap(newPosition) && IsMovablePoint(m_map, m_player, newPosition))
            {
                c.Position = newPosition;
                m_timingEngine.ActorMadeMove(c);
                m_fovManager.Update(this, m_map, m_player);    // Operating can change LOS and such
                didAnything = true;
            }
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
                    m_fovManager.Update(this, m_map, m_player);    // Operating can change LOS and such
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

        internal bool Attack(Character attacker, Direction direction)
        {
            bool didAnything = m_combatEngine.Attack(attacker, direction);
            if (didAnything)
            {
                m_timingEngine.ActorDidAction(attacker);
                m_fovManager.Update(this, m_map, m_player);
            }
            return didAnything;
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction(CoreGameEngine engine)
        {
            Character nextCharacter = m_timingEngine.GetNextActor(m_player, m_map);
            if (nextCharacter is Player)
                return;

            Monster monster = nextCharacter as Monster;
            monster.Action(engine);
        }
    }
}
