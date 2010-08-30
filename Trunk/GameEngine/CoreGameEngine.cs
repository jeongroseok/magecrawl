using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Magecrawl.Actors;
using Magecrawl.EngineInterfaces;
using Magecrawl.GameEngine.Interface;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.Physics;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Maps;
using Magecrawl.Maps.Generator;
using Magecrawl.Maps.Generator.Cave;
using Magecrawl.Maps.Generator.Stitch;
using Magecrawl.Maps.MapObjects;
using Magecrawl.StatusEffects.Interfaces;
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
        private MonsterSkillEffectEngine m_monsterSkillEngine;
        
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

        internal IMonsterSkillEngine MonsterSkillEngine
        {
            get
            {
                return m_monsterSkillEngine;
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

            m_saveLoad = new SaveLoadCore();

            m_timingEngine = new CoreTimingEngine();
            m_monsterSkillEngine = new MonsterSkillEffectEngine(this);

            m_dungeon = new Dictionary<int, Map>();

            StairsMapping.Setup();

            CoreGameEngineInterface.SetupCoreGameEngineInterface(this);
        }

        public void CreateNewWorld(string playerName, string startingBackground)
        {
            // Don't use property so we don't hit validation code
            m_currentLevel = 0;

            Stairs incommingStairs = null;
            Random random = new Random();
            bool generateCave = false;
            for (int i = 0; i < 5; ++i)
            {
                generateCave = random.Chance(50);
                MapGeneratorBase mapGenerator = generateCave ? (MapGeneratorBase)new SimpleCaveGenerator(random) : (MapGeneratorBase)new StitchtogeatherMapGenerator(random);
                    
                m_dungeon[i] = mapGenerator.GenerateMap(incommingStairs, i);

                incommingStairs = m_dungeon[i].MapObjects.Where(x => x.Type == MapObjectType.StairsDown).OfType<Stairs>().First();              
            }
 
            Point initialStairsUpPosition = m_dungeon[0].MapObjects.Where(x => x.Type == MapObjectType.StairsUp).OfType<Stairs>().First().Position;

            ClearMonstersFromStartingArea(initialStairsUpPosition);

            m_player = new Player(playerName, initialStairsUpPosition);
            PlayerBackgrounds.SetupBackground(m_player, startingBackground);

            TurnCount = 0;

            m_physicsEngine = new PhysicsEngine(Player, Map);
            m_pathFinding = new PathfindingMap(Player, Map);

            // If the player isn't the first actor, let others go. See archtecture note in PublicGameEngine.
            m_physicsEngine.AfterPlayerAction(this);
        }

        private void ClearMonstersFromStartingArea(Point initialStairsUpPosition)
        {
            List<Point> monsterFreePoints = PointListUtils.PointListFromBurstPosition(initialStairsUpPosition, 8);

            Random random = new Random();
            bool[,] moveablePoints = m_dungeon[0].CalculateMoveablePointGrid(true, initialStairsUpPosition);
            foreach(Monster monsterToMove in m_dungeon[0].Monsters.Where(m => monsterFreePoints.Contains(m.Position)).OfType<Monster>())
            {
                int attempts = 0;
                while (true)
                {
                    int possibleX = random.getInt(0, m_dungeon[0].Width - 1);
                    int possibleY = random.getInt(0, m_dungeon[0].Height - 1);
                    if (moveablePoints[possibleX, possibleY])
                    {
                        monsterToMove.Position = new Point(possibleX, possibleY);
                        moveablePoints[possibleX, possibleY] = false;
                        break;
                    }
                    else
                    {
                        attempts++;
                    }

                    // If we try 10000 random points and fail, throw an exception, since something is seriously wrong
                    if (attempts > 10000)
                        throw new MapGenerationFailureException("ClearMonstersFromStartingArea can't find spot to move monster");
                }
            }
        }

        public void LoadSaveFile(string saveGameName)
        {
            string saveGameDirectory = GetSaveDirectoryCreateIfNotExist();
            m_saveLoad.LoadGame(Path.Combine(saveGameDirectory, saveGameName));

            m_physicsEngine = new PhysicsEngine(Player, Map);
            m_pathFinding = new PathfindingMap(Player, Map);
        }

        public void Dispose()
        {
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

        internal bool UseMonsterSkill(Character invoker, MonsterSkillType skill, Point target)
        {
            return m_monsterSkillEngine.UseSkill(invoker, skill, target);
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

        internal bool ReloadWeapon(Player player)
        {
            return m_physicsEngine.ReloadWeapon(player);
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

        public bool CurrentOrRecentDanger()
        {
            return m_physicsEngine.CurrentOrRecentDanger();
        }

        public List<Character> MonstersInCharactersLOS(Character chacter)
        {
            List<Character> returnList = new List<Character>();
            FOVManager.CalculateForMultipleCalls(Map, chacter.Position, chacter.Vision);

            foreach (Monster m in Map.Monsters)
            {
                if (FOVManager.Visible(m.Position))
                    returnList.Add(m);
            }
            return returnList;
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
            List<ItemOptions> optionList = new List<ItemOptions>();

            if (item is IWeapon)
            {
                bool canEquip = m_player.CanEquipWeapon((IWeapon)item);
                optionList.Add(new ItemOptions("Equip", canEquip ));
                optionList.Add(new ItemOptions("Equip as Secondary", canEquip ));
            }
            if (item is IArmor)
            {
                optionList.Add(new ItemOptions("Equip", Player.CanEquipArmor((IArmor)item)));
            }

            if (item.ContainsAttribute("Invokable"))
                optionList.Add(new ItemOptions(item.GetAttribute("InvokeActionName"), true));

            optionList.Add(new ItemOptions("Drop", true));
            return optionList;
        }

        internal List<ItemOptions> GetOptionsForEquipmentItem(Item item)
        {
            List<ItemOptions> optionList = new List<ItemOptions>();

            // If the item isn't a secondary, we can unequip it.
            if (Player.SecondaryWeapon == item)
                optionList.Add(new ItemOptions("Unequip as Secondary", true));
            else            
                optionList.Add(new ItemOptions("Unequip", true));

            // TODO - Unsure how this'd work but it should be possible
            if (item.ContainsAttribute("Invokable"))
                optionList.Add(new ItemOptions(item.GetAttribute("InvokeActionName"), true));

            return optionList;
        }

        internal bool SwapPrimarySecondaryWeapons(Player player)
        {
            bool didSomething = player.SwapPrimarySecondaryWeapons();
            if (didSomething)
                m_timingEngine.ActorDidMinorAction(m_player);
            return didSomething;
        }

        internal bool PlayerSelectedItemOption(IItem item, string option, object argument)
        {
            bool didSomething = m_physicsEngine.HandleItemAction(item, option, argument);
            if (didSomething)
                m_timingEngine.ActorDidAction(m_player);
            return didSomething;
        }

        internal void FilterNotVisibleBothWaysFromList(Point centerPoint, List<EffectivePoint> pointList, Point pointToSaveFromList)
        {
            if (pointList == null)
                return;

            bool pointToSaveInList = pointList.Exists(x => x.Position == pointToSaveFromList);
            float effectiveStrengthAtPlayerPosition = pointToSaveInList ? pointList.First(x => x.Position == CoreGameEngine.Instance.Player.Position).EffectiveStrength : 0;

            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(centerPoint, x.Position));
            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(x.Position, centerPoint));

            if (pointToSaveInList)
                pointList.Add(new EffectivePoint(pointToSaveFromList, effectiveStrengthAtPlayerPosition));
        }

        internal void FilterNotVisibleBothWaysFromList(Point centerPoint, List<Point> pointList)
        {
            if (pointList == null)
                return;

            bool centerPointInList = pointList.Exists(x => x == centerPoint);

            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(centerPoint, x));
            pointList.RemoveAll(x => !IsRangedPathBetweenPoints(x, centerPoint));

            if (centerPointInList)
                pointList.Add(centerPoint);
        }

        internal ILongTermStatusEffect GetLongTermEffectSpellWouldProduce(string effectName)
        {
            return m_physicsEngine.GetLongTermEffectSpellWouldProduce(effectName);
        }
    }
}
