using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Level;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class PhysicsEngine : IDisposable
    {
        private CoreTimingEngine m_timingEngine;
        private FOVManager m_fovManager;
        private CombatEngine m_combatEngine;
        private MagicEffectsEngine m_magicEffects;
        private SkillEffectEngine m_skillEngine;

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
            m_fovManager = new FOVManager(this, map);
            m_combatEngine = new CombatEngine(player, map);
            m_movableHash = new Dictionary<Point, bool>();
            m_magicEffects = new MagicEffectsEngine(this, m_combatEngine);
            m_skillEngine = new SkillEffectEngine(this, m_combatEngine);
            UpdatePlayerVisitedStatus();
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

        internal void NewMapPlayerInfo(Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_combatEngine.NewMapPlayerInfo(player, map);

            // We have a new map, recalc LOS with a new map
            m_fovManager.UpdateNewMap(this, m_map);
            UpdatePlayerVisitedStatus();
        }

        // This needs to be really _fast_. We're going to stick the not moveable points in a has table,
        // then compare each pointList to the terrian and if still good see if in hash table
        public void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, Point characterPosition, int visionRange, bool needsToBeVisible)
        {
            m_fovManager.CalculateForMultipleCalls(m_map, characterPosition, visionRange);
            m_movableHash.Clear();

            foreach (MapObject obj in m_map.MapObjects)
            {
                if (obj.IsSolid)
                    m_movableHash[obj.Position] = true;
            }

            // Remove it if it's not on map, or is wall, or same square as something solid from above, is it's not visible.
            pointList.RemoveAll(point => 
                !m_map.IsPointOnMap(point.Position) || 
                m_map.GetTerrainAt(point.Position) == TerrainType.Wall || 
                m_movableHash.ContainsKey(point.Position) ||
                (needsToBeVisible && !m_fovManager.Visible(point.Position)));
        }

        // There are many times we want to know what cells are movable into, for FOV or Pathfinding for example
        // This calculates them in batch much much more quickly. True means you can walk there.
        internal static bool[,] CalculateMoveablePointGrid(Map map, Point characterPosition, bool monstersBlockPath)
        {
            return CalculateMoveablePointGrid(map, characterPosition, new Point(0, 0), map.Width, map.Height, monstersBlockPath);
        }

        internal static bool[,] CalculateMoveablePointGrid(Map map, Point characterPosition, Point upperLeftCorner, int width, int height, bool monstersBlockPath)
        {
            bool[,] returnValue = CalculateMoveablePointGrid(map, upperLeftCorner, width, height, monstersBlockPath);

            returnValue[characterPosition.X, characterPosition.Y] = false;

            return returnValue;
        }

        internal static bool[,] CalculateMoveablePointGrid(Map map, bool monstersBlockPath)
        {
            return CalculateMoveablePointGrid(map, new Point(0, 0), map.Width, map.Height, monstersBlockPath);
        }

        // Returns an array the full size of map, but only with the requested part filled in. This is done for ease of use.
        // We can use (x,y), instead of (x-offset, y-offset) to get data.
        internal static bool[,] CalculateMoveablePointGrid(Map map, Point upperLeftCorner, int width, int height, bool monstersBlockPath)
        {
            bool[,] returnValue = new bool[map.Width, map.Height];

            for (int i = upperLeftCorner.X; i < upperLeftCorner.X + width; ++i)
            {
                for (int j = upperLeftCorner.Y; j < upperLeftCorner.Y + height; ++j)
                {
                    returnValue[i, j] = map.GetTerrainAt(new Point(i, j)) == TerrainType.Floor;
                }
            }

            foreach (MapObject obj in map.MapObjects.Where(x => x.IsSolid))
            {
                returnValue[obj.Position.X, obj.Position.Y] = false;
            }

            if (monstersBlockPath)
            {
                foreach (Monster m in map.Monsters)
                {
                    returnValue[m.Position.X, m.Position.Y] = false;
                }
            }

            return returnValue;
        }

        // This is a slow operation. It should not be called multiple times in a row!
        // Call CalculateMoveablePointGrid instead~!
        private bool IsMovablePoint(Map map, Player player, Point p)
        {
            // If it's not a floor, it's not movable
            if (map.GetTerrainAt(p) != TerrainType.Floor)
                return false;

            // If there's a map object there that is solid, it's not movable
            if (map.MapObjects.SingleOrDefault(m => m.Position == p && m.IsSolid) != null)
                return false;

            // If there's a monster there, it's not movable
            if (map.Monsters.SingleOrDefault(m => m.Position == p) != null)
                return false;

            // If the player is there, it's not movable
            if (player.Position == p)
                return false;

            return true;
        }

        public TileVisibility[,] CalculateTileVisibility()
        {
            TileVisibility[,] visibilityArray = new TileVisibility[m_map.Width, m_map.Height];

            m_fovManager.CalculateForMultipleCalls(m_map, m_player.Position, m_player.Vision);

            for (int i = 0; i < m_map.Width; ++i)
            {
                for (int j = 0; j < m_map.Height; ++j)
                {
                    Point p = new Point(i, j);
                    if (m_fovManager.Visible(p))
                    {
                        visibilityArray[i, j] = TileVisibility.Visible;
                    }
                    else
                    {
                        if (m_map.IsVisitedAt(p))
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
                c.Position = newPosition;
                m_timingEngine.ActorMadeMove(c);
                didAnything = true;
            }
            return didAnything;
        }

        internal bool WarpToPosition(Character c, Point p)
        {
            c.Position = p;
            return true;
        }

        private void UpdatePlayerVisitedStatus()
        {
            m_fovManager.CalculateForMultipleCalls(m_map, m_player.Position, m_player.Vision);

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
                    Point p = new Point(i, j);
                    if (m_map.IsPointOnMap(p) && m_fovManager.Visible(p))
                        m_map.SetVisitedAt(p, true);
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


        public bool PlayerReadScroll(Scroll scroll)
        {
            if (m_player.Items.Contains(scroll))
            {
                m_player.RemoveItem(scroll);
                m_magicEffects.ReadScroll(m_player, scroll);
                return true;
            }
            return false;
        }

        public bool Operate(Character characterOperating, Point pointToOperateAt)
        {
            OperableMapObject operateObj = m_map.MapObjects.OfType<OperableMapObject>().SingleOrDefault(x => x.Position == pointToOperateAt);
            if (operateObj != null)
            {
                operateObj.Operate();
                m_timingEngine.ActorDidAction(characterOperating);
                return true;
            }
            return false;
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
                m_timingEngine.ActorDidWeaponAttack(attacker);
            return didAnything;
        }

        internal bool CastSpell(Player caster, Spell spell, Point target)
        {
            bool didAnything = m_magicEffects.CastSpell(caster, spell, target);
            if (didAnything)
                m_timingEngine.ActorDidAction(caster);
            return didAnything;
        }

        internal bool UseSkill(Character attacker, SkillType skill, Point target)
        {
            bool didAnything = m_skillEngine.UseSkill(attacker, skill, target);
            if (didAnything)
                m_timingEngine.ActorDidAction(attacker);
            return didAnything;
        }

        internal bool ReloadWeapon(Character character)
        {
            ((WeaponBase)character.CurrentWeapon).IsLoaded = true;
            m_timingEngine.ActorDidMinorAction(character);
            return true;
        }

        internal bool PlayerMoveUpStairs(Player player, Map map)
        {
            Stairs s = map.MapObjects.OfType<Stairs>().Where(x => x.Type == MapObjectType.StairsUp && x.Position == player.Position).SingleOrDefault();

            if (s != null)
            {
                // The position must come first, as changing levels checks FOV
                m_player.Position = StairsMapping.Instance.GetMapping(s.UniqueID);
                CoreGameEngine.Instance.CurrentLevel--;
                
                m_timingEngine.ActorMadeMove(m_player);
                return true;
            }
            return false;
        }

        internal bool PlayerMoveDownStairs(Player player, Map map)
        {
            Stairs s = map.MapObjects.OfType<Stairs>().Where(x => x.Type == MapObjectType.StairsDown && x.Position == player.Position).SingleOrDefault();
            if (s != null)
            {
                if (CoreGameEngine.Instance.CurrentLevel == CoreGameEngine.Instance.NumberOfLevels - 1)
                    throw new InvalidOperationException("Win dialog should have come up instead.");
                
                // The position must come first, as changing levels checks FOV
                m_player.Position = StairsMapping.Instance.GetMapping(s.UniqueID);
                CoreGameEngine.Instance.CurrentLevel++;

                m_timingEngine.ActorMadeMove(m_player);
                return true;
            }
            return false;
        }

        // Called by PublicGameEngine after any call that could pass time.
        internal void BeforePlayerAction(CoreGameEngine engine)
        {
            UpdatePlayerVisitedStatus();
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction(CoreGameEngine engine)
        {
            UpdatePlayerVisitedStatus();
            
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
