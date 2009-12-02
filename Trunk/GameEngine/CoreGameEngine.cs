using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Level;
using Magecrawl.GameEngine.Level.Generator;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    // This class mostly coordinates between a bunch of helper classes to provide what PublicGameEngine needs.
    internal sealed class CoreGameEngine : IDisposable
    {
        private Player m_player;

        private Dictionary<int, Map> m_dungeon;

        private SaveLoadCore m_saveLoad;
        private PathfindingMap m_pathFinding;
        private PhysicsEngine m_physicsEngine;
        private CoreTimingEngine m_timingEngine;
        
        internal ItemFactory ItemFactory;
        internal MonsterFactory MonsterFactory;
        internal MapObjectFactory MapObjectFactory;

        private event PlayerDiedDelegate m_playerDied;
        private event TextOutputFromGame m_textOutput;

        // Almost every member of the GameEngine component will need to call CoreGameEngine at some point.
        // As opossed to have everyone stash a copy of it, just make it a singleton.
        private static CoreGameEngine m_instance;
        public static CoreGameEngine Instance
        {
            get
            {
                return m_instance;
            }
        }

        public CoreGameEngine(TextOutputFromGame textOutput, PlayerDiedDelegate diedDelegate)
        {
            CommonStartup(textOutput, diedDelegate);

            // Don't use property so we don't hit validation code
            m_currentLevel = 0;
            Point playerPosition = new Point(35, 35);
            m_player = new Player(playerPosition);

            Point stairsUpPosition = playerPosition;
            Point stairsDownPosition;

            using (TCODRandom random = new TCODRandom())
            {
                for (int i = 0; i < 5; ++i)
                {
                    MapGeneratorBase mapGenerator = null;
                    try
                    {
                        if (random.Chance(50))
                            mapGenerator = new SimpleCaveGenerator(random);
                        else
                            mapGenerator = new StitchtogeatherMapGenerator(random);
                        m_dungeon[i] = mapGenerator.GenerateMap(stairsUpPosition, out stairsDownPosition);
                        stairsUpPosition = stairsDownPosition;
                        stairsDownPosition = Point.Invalid;
                    }
                    finally
                    {
                        if (mapGenerator == null)
                            mapGenerator.Dispose();
                    }
                }
            }

            CommonStartupAfterMapPlayer();

            SendTextOutput("Welcome To Magecrawl.");
        }

        public CoreGameEngine(TextOutputFromGame textOutput, PlayerDiedDelegate diedDelegate, string saveGameName)
        {
            CommonStartup(textOutput, diedDelegate);

            m_saveLoad.LoadGame(saveGameName);

            CommonStartupAfterMapPlayer();

            SendTextOutput("Welcome Back To Magecrawl");
        }

        private void CommonStartup(TextOutputFromGame textOutput, PlayerDiedDelegate diedDelegate)
        {
            m_instance = this;
            m_playerDied += diedDelegate;
            m_textOutput += textOutput;

            // Needs to happen before anything that could create a weapon
            ItemFactory = new ItemFactory();
            MonsterFactory = new MonsterFactory();
            MapObjectFactory = new MapObjectFactory();

            m_saveLoad = new SaveLoadCore();
            
            m_timingEngine = new CoreTimingEngine();

            m_dungeon = new Dictionary<int, Map>();
        }

        private void CommonStartupAfterMapPlayer()
        {
            m_physicsEngine = new PhysicsEngine(Player, Map);
            m_pathFinding = new PathfindingMap(Player, Map);

            // If the player isn't the first actor, let others go. See archtecture note in PublicGameEngine.
            m_physicsEngine.AfterPlayerAction(this);
        }

        public void Dispose()
        {
            if (m_physicsEngine != null)
                m_physicsEngine.Dispose();
            m_physicsEngine = null;

            if (m_pathFinding != null)
                m_pathFinding.Dispose();
            m_pathFinding = null;

            m_instance = null;
        }

        internal CombatEngine CombatEngine
        {
            get
            {
                return m_physicsEngine.CombatEngine;
            }
        }

        internal Player Player
        {
            get
            {
                return m_player;
            }
        }

        internal Map Map
        {
            get
            {
                return m_dungeon[CurrentLevel];
            }
        }

        private int m_currentLevel;
        public int CurrentLevel
        {
            get
            {
                return m_currentLevel;
            }
            internal set
            {
                m_currentLevel = value;

                m_pathFinding.Dispose();
                m_pathFinding = new PathfindingMap(Player, Map);
                m_physicsEngine.NewMapPlayerInfo(Player, Map);
            }
        }

        internal Map GetSpecificFloor(int i)
        {
            return m_dungeon[i];
        }

        internal int NumberOfLevels
        {
            get
            {
                return m_dungeon.Keys.Count;
            }
        }

        // Due to the way XML serialization works, we grab the Instance version of this 
        // class, not any that we could pass in. This allows us to set the map data.
        internal void SetWithSaveData(Player p, Dictionary<int, Map> d, int currentLevel)
        {
            m_player = p;
            m_dungeon = d;

            // Don't use property so we don't hit state changing code
            m_currentLevel = currentLevel;
        }

        internal void Save()
        {
            SendTextOutput("Saving Game.");
            m_saveLoad.SaveGame(m_player.Name + ".sav");
        }

        internal bool Move(Character c, Direction direction)
        {
            return m_physicsEngine.Move(c, direction);
        }

        internal bool Attack(Character attacker, Point target)
        {
            return m_physicsEngine.Attack(attacker, target);
        }

        internal bool CastSpell(Character caster, Spell spell, Point target)
        {
            return m_physicsEngine.CastSpell(caster, spell, target);
        }

        public bool Operate(Character characterOperating, Point pointToOperateAt)
        {
            // Right now, you can only operate next to a thing
            if (PointDirectionUtils.LatticeDistance(characterOperating.Position, pointToOperateAt) != 1)
                return false;
            return m_physicsEngine.Operate(characterOperating, pointToOperateAt);
        }

        internal bool Wait(Character c)
        {
            return m_physicsEngine.Wait(c);
        }

        internal bool PlayerGetItem()
        {
            return m_physicsEngine.PlayerGetItem();
        }

        internal bool PlayerMoveUpStairs()
        {
            Stairs s = Map.MapObjects.OfType<Stairs>().Where(x => x.Type == MapObjectType.StairsUp && x.Position == Player.Position).SingleOrDefault();
            if (s != null)
            {
                CurrentLevel--;
                m_timingEngine.ActorMadeMove(m_player);
                return true;
            }
            return false;
        }

        internal bool PlayerMoveDownStairs()
        {
            Stairs s = Map.MapObjects.OfType<Stairs>().Where(x => x.Type == MapObjectType.StairsDown && x.Position == Player.Position).SingleOrDefault();
            if (s != null)
            {
                CurrentLevel++;
                m_timingEngine.ActorMadeMove(m_player);
                return true;
            }
            return false;
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction()
        {
            m_physicsEngine.AfterPlayerAction(this);
        }

        internal List<Point> PathToPoint(Character actor, Point dest, bool canOperate)
        {
            return m_pathFinding.Travel(actor, dest, canOperate, m_physicsEngine);
        }

        // This is used by the Movability Debug View. If you think you need it, you don't. Talk to chris. 
        // Unless your me, this smack yourself and try again.
        public bool[,] PlayerMoveableToEveryPoint()
        {
            return PhysicsEngine.CalculateMoveablePointGrid(Map, Player.Position);
        }

        public void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, Point characterPosition, int visionRange, bool needsToBeVisible)
        {
            m_physicsEngine.FilterNotTargetablePointsFromList(pointList, characterPosition, visionRange, needsToBeVisible);
        }

        public TileVisibility[,] CalculateTileVisibility()
        {
            return m_physicsEngine.CalculateTileVisibility();
        }

        internal void PlayerDied()
        {
            m_playerDied();
        }

        internal void SendTextOutput(string s)
        {
            m_textOutput(s);
        }            

        internal FOVManager FOVManager
        {
            get
            {
                return m_physicsEngine.FOVManager;
            }
        }

        internal List<ItemOptions> GetOptionsForInventoryItem(Item item)
        {
            return item.PlayerOptions;
        }

        internal List<ItemOptions> GetOptionsForEquipmentItem(Item item)
        {
            return item.PlayerOptions;
        }

        // TODO - This should be somewhere else
        internal bool PlayerSelectedItemOption(IItem item, string option)
        {
            bool didSomething = false;
            switch (option)
            {
                case "Drop":
                    didSomething = m_physicsEngine.PlayerDropItem(item as Item);
                    break;
                case "Equip": 
                {
                    // This probally should live in the player code
                    m_player.RemoveItem(item as Item);
                    Item oldWeapon = m_player.EquipWeapon(item as IWeapon) as Item;
                    if (oldWeapon != null)
                        m_player.TakeItem(oldWeapon);
                    didSomething = true;
                    break;
                }
                case "Unequip":
                {
                    Item oldWeapon = m_player.UnequipWeapon() as Item;
                    if (oldWeapon != null)
                        m_player.TakeItem(oldWeapon);
                    didSomething = true;
                    break;
                }
                case "Drink":
                    didSomething = m_physicsEngine.PlayerDrinkPotion(item as Potion);
                    break;
            }
            if (didSomething)
                m_timingEngine.ActorDidAction(m_player);
            return didSomething;
        }
    }
}
