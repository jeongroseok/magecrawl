using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Level;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class PhysicsEngine : IDisposable
    {
        private CoreTimingEngine m_timingEngine;
        private FOVManager m_fovManager;
        private CombatEngine m_combatEngine;
        private MagicEffectsEngine m_magicEffects;

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
            m_magicEffects = new MagicEffectsEngine(this, m_combatEngine);
        }

        public void Dispose()
        {
            if (m_fovManager != null)
                m_fovManager.Dispose();
            m_fovManager = null;
        }

        internal CombatEngine CombatEngine
        {
            get { return m_combatEngine; }
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
        public void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, Point characterPosition, int visionRange, bool needsToBeVisible)
        {
            m_fovManager.CalculateForMultipleCalls(characterPosition, visionRange);
            m_movableHash.Clear();

            foreach (MapObject obj in m_map.MapObjects)
            {
                if (obj.IsSolid)
                    m_movableHash[obj.Position] = true;
            }

            // Remove it if it's not on map, or is wall, or same square as something solid from above, is it's not visible.
            pointList.RemoveAll(point => 
                !m_map.IsPointOnMap(point.Position) || 
                m_map[point.Position.X, point.Position.Y].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Wall || 
                m_movableHash.ContainsKey(point.Position) ||
                (needsToBeVisible && !m_fovManager.Visible(point.Position)));
        }

        // There are many times we want to know what cells are movable into, for FOV or Pathfinding for example
        // This calculates them in batch much much more quickly. True means you can walk there.
        internal static bool[,] CalculateMoveablePointGrid(Map map, Player player)
        {
            bool[,] returnValue = CalculateMoveablePointGrid(map);

            returnValue[player.Position.X, player.Position.Y] = false;

            return returnValue;
        }

        internal static bool[,] CalculateMoveablePointGrid(Map map)
        {
            bool[,] returnValue = new bool[map.Width, map.Height];

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    returnValue[i, j] = map[i, j].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Floor;
                }
            }

            foreach (MapObject obj in map.MapObjects)
            {
                if (obj.IsSolid)
                {
                    returnValue[obj.Position.X, obj.Position.Y] = false;
                }
            }

            foreach (Monster m in map.Monsters)
            {
                returnValue[m.Position.X, m.Position.Y] = false;
            }

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

        public TileVisibility[,] CalculateTileVisibility()
        {
            TileVisibility[,] visibilityArray = new TileVisibility[m_map.Width, m_map.Height];

            m_fovManager.CalculateForMultipleCalls(m_player.Position, m_player.Vision);

            for (int i = 0; i < m_map.Width; ++i)
            {
                for (int j = 0; j < m_map.Height; ++j)
                {
                    if (m_fovManager.Visible(new Point(i, j)))
                    {
                        visibilityArray[i, j] = TileVisibility.Visible;
                    }
                    else
                    {
                        if (m_map[i, j].Visited)
                            visibilityArray[i, j] = TileVisibility.Visited;
                        else
                            visibilityArray[i, j] = TileVisibility.Unvisited;
                    }
                }
            }
            return visibilityArray;
        }

        internal bool Move(Character c, Direction direction)
        {
            bool didAnything = false;
            Point newPosition = PointDirectionUtils.ConvertDirectionToDestinationPoint(c.Position, direction);
            if (m_map.IsPointOnMap(newPosition) && IsMovablePoint(m_map, m_player, newPosition))
            {
                UpdatePlayerVisitedStatus();
                c.Position = newPosition;
                m_timingEngine.ActorMadeMove(c);
                m_fovManager.Update(this, m_map, m_player);
                UpdatePlayerVisitedStatus();
                didAnything = true;
            }
            return didAnything;
        }

        internal bool WarpToPosition(Character c, Point p)
        {
            UpdatePlayerVisitedStatus();
            c.Position = p;
            UpdatePlayerVisitedStatus();
            return true;
        }

        private void UpdatePlayerVisitedStatus()
        {
            m_fovManager.CalculateForMultipleCalls(m_player.Position, m_player.Vision);

            // Only need Vision really, but to catch off by one errors and such, make it bigger
            // We're doing this instead of all cells for performance anyway
            int minX = m_player.Position.X - (m_player.Vision * 2);
            int minY = m_player.Position.Y - (m_player.Vision * 2);
            int maxX = m_player.Position.X + (m_player.Vision * 2);
            int maxY = m_player.Position.Y + (m_player.Vision * 2);

            for (int i = minX; i < maxX; ++i)
            {
                for (int j = minY; j < maxY; ++j)
                {
                    if (m_map.IsPointOnMap(new Point(i, j)))
                    {
                        if (m_fovManager.Visible(new Point(i, j)))
                        {
                            m_map.GetInternalTile(i, j).Visited = true;
                        }
                    }
                }
            }
        }

        public bool PlayerGetItem()
        {
            foreach (Pair<Item, Point> i in m_map.InternalItems)
            {
                if (m_player.Position == i.Second)
                {
                    m_map.RemoveItem(i);
                    m_player.TakeItem(i.First);
                    m_timingEngine.ActorDidAction(m_player);
                    CoreGameEngine.Instance.SendTextOutput(string.Format("Picked up a {0}.", i.First.DisplayName));
                    return true;
                }
            }

            return false;
        }

        public bool PlayerDropItem(Item item)
        {
            if (m_player.Items.Contains(item))
            {
                m_map.AddItem(new Pair<Item, Point>(item, m_player.Position));
                m_player.RemoveItem(item);
                return true;
            }
            return false;
        }

        public bool PlayerDrinkPotion(Potion potion)
        {
            if (m_player.Items.Contains(potion))
            {
                m_player.RemoveItem(potion);
                m_magicEffects.DrinkPotion(m_player, potion);
                return true;
            }
            return false;
        }

        public bool Operate(Character characterOperating, Point pointToOperateAt)
        {
            bool didAnything = false;

            foreach (MapObject obj in m_map.MapObjects)
            {
                OperableMapObject operateObj = obj as OperableMapObject;
                if (operateObj != null && operateObj.Position == pointToOperateAt)
                {
                    operateObj.Operate();
                    m_timingEngine.ActorDidAction(characterOperating);
                    m_fovManager.Update(this, m_map, m_player);
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

        internal bool CastSpell(Character caster, Spell spell, Point target)
        {
            bool didAnything = m_magicEffects.CastSpell(caster, spell, target);
            if (didAnything)
            {
                m_timingEngine.ActorDidAction(caster);
                m_fovManager.Update(this, m_map, m_player);
            }
            return didAnything;
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
            }
        }
    }
}
