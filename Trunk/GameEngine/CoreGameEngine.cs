using System;
using System.Collections.Generic;
using System.IO;
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
        internal int TurnCount { get; set; }

        internal event TextOutputFromGame TextOutputEvent;
        internal event PlayerDied PlayerDiedEvent;
        internal event RangedAttack RangedAttackEvent;

        internal FOVManager FOVManager
        {
            get
            {
                return m_physicsEngine.FOVManager;
            }
        }

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

        public CoreGameEngine()
        {
            m_instance = this;

            // Needs to happen before anything that could create a weapon
            ItemFactory = new ItemFactory();
            MonsterFactory = new MonsterFactory();
            MapObjectFactory = new MapObjectFactory();

            m_saveLoad = new SaveLoadCore();

            m_timingEngine = new CoreTimingEngine();

            m_dungeon = new Dictionary<int, Map>();

            StairsMapping.Setup();
        }

        public void CreateNewWorld(string playerName)
        {
            // Don't use property so we don't hit validation code
            m_currentLevel = 0;
            Point playerPosition = Point.Invalid;

            int failedMapCreationAttempts = 0;
            Stairs incommingStairs = null;
            using (TCODRandom random = new TCODRandom())
            {
                bool generateCave = false;
                for (int i = 0; i < 5; ++i)
                {
                    if (failedMapCreationAttempts == 0)
                        generateCave = random.Chance(50);
                    MapGeneratorBase mapGenerator = null;
                    try
                    {
                        if (generateCave)
                            mapGenerator = new SimpleCaveGenerator(random);
                        else
                            mapGenerator = new StitchtogeatherMapGenerator(random);
                        m_dungeon[i] = mapGenerator.GenerateMap(incommingStairs);

                        incommingStairs = m_dungeon[i].MapObjects.Where(x => x.Type == MapObjectType.StairsDown).OfType<Stairs>().First();

                        // We succeeded in creating a good map, reset attempts
                        failedMapCreationAttempts = 0;
                    }
                    catch (MapGenerationFailureException)
                    {
                        // Let's try again.
                        if (failedMapCreationAttempts < 20)
                        {
                            i--;
                            failedMapCreationAttempts++;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            Point initialStairsUpPosition = m_dungeon[0].MapObjects.Where(x => x.Type == MapObjectType.StairsUp).OfType<Stairs>().First().Position;
            m_player = new Player(playerName, initialStairsUpPosition);

            TurnCount = 0;

            CommonStartupAfterMapPlayer();

            SendTextOutput("Welcome To Magecrawl.");
            SendTextOutput("If this is your first time, press '?' for help.");
        }

        public void LoadSaveFile(string saveGameName)
        {
            string saveGameDirectory = GetSaveDirectoryCreateIfNotExist();
            m_saveLoad.LoadGame(Path.Combine(saveGameDirectory, saveGameName));

            CommonStartupAfterMapPlayer();
            SendTextOutput("Welcome Back To Magecrawl");
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
            string saveGameDirectory = GetSaveDirectoryCreateIfNotExist();

            m_saveLoad.SaveGame(Path.Combine(saveGameDirectory, m_player.Name) + ".sav");
        }

        private static string GetSaveDirectoryCreateIfNotExist()
        {
            string saveGameDirectory = Path.Combine(AssemblyDirectory.CurrentAssemblyDirectory, "Saves");
            if (!Directory.Exists(saveGameDirectory))
                Directory.CreateDirectory(saveGameDirectory);
            return saveGameDirectory;
        }

        internal bool Move(Character c, Direction direction)
        {
            return m_physicsEngine.Move(c, direction);
        }

        internal bool Attack(Character attacker, Point target)
        {
            return m_physicsEngine.Attack(attacker, target);
        }

        internal bool CastSpell(Player caster, Spell spell, Point target)
        {
            return m_physicsEngine.CastSpell(caster, spell, target);
        }

        internal List<Point> TargettedDrawablePoints(object targettingObject, Point target)
        {
            return m_physicsEngine.TargettedDrawablePoints(targettingObject, target);
        }

        internal bool UseSkill(Character attacker, SkillType skill, Point target)
        {
            return m_physicsEngine.UseSkill(attacker, skill, target);
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

        internal bool ReloadWeapon(Character character)
        {
            return m_physicsEngine.ReloadWeapon(character);
        }

        internal bool PlayerGetItem()
        {
            return m_physicsEngine.PlayerGetItem();
        }

        internal bool PlayerGetItem(IItem item)
        {
            return m_physicsEngine.PlayerGetItem(item);
        }

        internal bool PlayerMoveUpStairs()
        {
            return m_physicsEngine.PlayerMoveUpStairs(Player, Map);
        }

        internal bool PlayerMoveDownStairs()
        {
            return m_physicsEngine.PlayerMoveDownStairs(Player, Map);
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction()
        {
            m_physicsEngine.AfterPlayerAction(this);
            TurnCount++;
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void BeforePlayerAction()
        {
            m_physicsEngine.BeforePlayerAction(this);
        }

        internal List<Point> PathToPoint(Character actor, Point dest, bool canOperate, bool usePlayerLOS, bool monstersBlockPath)
        {
            return m_pathFinding.Travel(actor, dest, canOperate, m_physicsEngine, usePlayerLOS, monstersBlockPath);
        }

        public bool IsRangedPathBetweenPoints(Point x, Point y)
        {
            return m_physicsEngine.IsRangedPathBetweenPoints(x, y);
        }

        // This is used by the Movability Debug View. If you think you need it, you don't. Talk to Chris. 
        // Unless you are Chris, then smack yourself and try again.
        public bool[,] PlayerMoveableToEveryPoint()
        {
            return PhysicsEngine.CalculateMoveablePointGrid(Map, Player.Position, true);
        }

        public void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, Point characterPosition, int visionRange, bool needsToBeVisible)
        {
            m_physicsEngine.FilterNotTargetablePointsFromList(pointList, characterPosition, visionRange, needsToBeVisible);
        }

        public void FilterNotTargetablePointsFromList(List<Point> pointList, Point characterPosition, int visionRange, bool needsToBeVisible)
        {
            m_physicsEngine.FilterNotTargetablePointsFromList(pointList, characterPosition, visionRange, needsToBeVisible);
        }

        public TileVisibility[,] CalculateTileVisibility()
        {
            return m_physicsEngine.CalculateTileVisibility();
        }

        internal void PlayerDied()
        {
            PlayerDiedEvent();
        }

        internal void SendTextOutput(string s)
        {
            TextOutputEvent(s);
        }

        internal void ShowRangedAttack(object attackingMethod, ShowRangedAttackType type, object data, bool targetAtEndPoint)
        {
            RangedAttackEvent(attackingMethod, type, data, targetAtEndPoint);
        }

        public bool DangerPlayerInLOS()
        {
            return m_physicsEngine.DangerPlayerInLOS();
        }

        public List<ICharacter> MonstersInPlayerLOS()
        {
            List<ICharacter> returnList = new List<ICharacter>();
            FOVManager.CalculateForMultipleCalls(Map, Player.Position, Player.Vision);

            foreach (Monster m in Map.Monsters)
            {
                if (FOVManager.Visible(m.Position))
                    returnList.Add(m);
            }
            return returnList;
        }

        internal List<ItemOptions> GetOptionsForInventoryItem(Item item)
        {
            return item.PlayerOptions;
        }

        internal List<ItemOptions> GetOptionsForEquipmentItem(Item item)
        {
            return item.PlayerOptions;
        }

        internal bool SwapPrimarySecondaryWeapons(Character character)
        {
            // If we're swapping to Melee and we're Melee
            if (character.CurrentWeapon.GetType() == typeof(MeleeWeapon) && character.SecondaryWeapon.GetType() == typeof(MeleeWeapon))
                return false;

            IWeapon mainWeapon = (IWeapon)character.Unequip(character.CurrentWeapon);
            IWeapon secondaryWeapon = character.UnequipSecondaryWeapon();
            
            if (secondaryWeapon != null)
                character.Equip(secondaryWeapon);
            
            if (mainWeapon != null)
                character.EquipSecondaryWeapon(mainWeapon);

            m_timingEngine.ActorDidMinorAction(character);
            return true;
        }

        internal bool PlayerSelectedItemOption(IItem item, string option, object argument)
        {
            bool didSomething = m_physicsEngine.HandleInventoryAction(item, option, argument);
            if (didSomething)
                m_timingEngine.ActorDidAction(m_player);
            return didSomething;
        }

        internal void FilterNotVisibleBothWaysFromList(Point centerPoint, List<EffectivePoint> pointList)
        {
            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(centerPoint, x.Position));
            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(x.Position, centerPoint));
        }

        internal void FilterNotVisibleBothWaysFromList(Point centerPoint, List<Point> pointList)
        {
            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(centerPoint, x));
            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(x, centerPoint));
        }
    }
}
