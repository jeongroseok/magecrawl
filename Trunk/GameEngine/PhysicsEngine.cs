using System;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;
using System.Collections.Generic;

namespace Magecrawl.GameEngine
{
    internal sealed class PhysicsEngine : IDisposable
    {
        private CoreTimingEngine m_timingEngine;
        private FOVManager m_fovManager;
        private CombatEngine m_combatEngine;

        // Fov FilterNotMovablePointsFromList
        private Dictionary<Point, bool> m_movableHash;

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
            m_movableHash = new Dictionary<Point, bool>();
        }

        public CombatEngine CombatEngine
        {
            get
            {
                return m_combatEngine;
            }
        }

        public void Dispose()
        {
            if (m_fovManager != null)
                m_fovManager.Dispose();
            m_fovManager = null;
        }

        internal FOVManager FOVManager
        {
            get
            {
                return m_fovManager;
            }
        }

        internal void GameLoaded(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_combatEngine.GameLoaded(player, map);

            // We have a new map, recalc LOS with a new map
            m_fovManager.UpdateNewMap(this, m_map, player);    
        }

        // This needs to be really _fast_. We're going to stick the not moveable points in a has table,
        // then compare each pointList to the terrian and if still good see if in hash table
        public void FilterNotTargetablePointsFromList(List<WeaponPoint> pointList)
        {
            m_movableHash.Clear();

            foreach (MapObject obj in m_map.MapObjects)
            {
                if (obj.IsSolid)
                    m_movableHash[obj.Position] = true;
            }

            // Remove it if it's not on map, or is wall, or same square as something solid from above
            pointList.RemoveAll(point => 
                !m_map.IsPointOnMap(point.Position) || 
                m_map[point.Position.X, point.Position.Y].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Wall || 
                m_movableHash.ContainsKey(point.Position));
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
        private bool IsMovablePoint(Map map, Player player, Point p)
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

        internal bool Attack(Character attacker, Point target)
        {
            bool didAnything = m_combatEngine.Attack(attacker, target);
            if (didAnything)
            {
                m_timingEngine.ActorDidAction(attacker);
                m_fovManager.Update(this, m_map, m_player);
            }
            return didAnything;
        }

        internal void CastSpell(CoreGameEngine engine, Character attacker, SpellBase spell)
        {
            spell.Cast(attacker, engine, this.m_combatEngine);
            m_timingEngine.ActorDidAction(attacker);
            m_fovManager.Update(this, m_map, m_player);
        }
        
        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction(CoreGameEngine engine)
        {
            // Until the player gets a turn
            while (true)
            {
                Character nextCharacter = m_timingEngine.GetNextActor(m_player, m_map);
                if (nextCharacter is Player)
                    return;
                Monster monster = nextCharacter as Monster;
                monster.Action(engine);
                nextCharacter = m_timingEngine.GetNextActor(m_player, m_map);
            }
        }
    }
}
