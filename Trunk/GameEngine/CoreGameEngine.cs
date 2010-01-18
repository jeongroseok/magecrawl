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
        internal uint TurnCount;

        private event PlayerDiedDelegate m_playerDied;
        private event TextOutputFromGame m_textOutput;
        private event RangedAttackAgainstPlayer m_rangeOnPlayer;

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

        public CoreGameEngine(TextOutputFromGame textOutput, PlayerDiedDelegate playerDiedDelegate, RangedAttackAgainstPlayer rangedAttack)
        {
            CommonStartup(textOutput, playerDiedDelegate, rangedAttack);

            // Don't use property so we don't hit validation code
            m_currentLevel = 0;
            Point playerPosition = Point.Invalid;

            int failedMapCreationAttempts = 0;
            Stairs incommingStairs = null;
            using (TCODRandom random = new TCODRandom())
            {
                for (int i = 0; i < 10; ++i)
                {
                    MapGeneratorBase mapGenerator = null;
                    try
                    {
                        if (random.Chance(50))
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
                        if (failedMapCreationAttempts < 10)
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
            m_player = new Player(initialStairsUpPosition);

            TurnCount = 0;

            CommonStartupAfterMapPlayer();

            SendTextOutput("Welcome To Magecrawl.");
        }

        public CoreGameEngine(TextOutputFromGame textOutput, PlayerDiedDelegate playerDiedDelegate, RangedAttackAgainstPlayer rangedAttack, string saveGameName)
        {
            CommonStartup(textOutput, playerDiedDelegate, rangedAttack);

            m_saveLoad.LoadGame(saveGameName);

            CommonStartupAfterMapPlayer();

            SendTextOutput("Welcome Back To Magecrawl");
        }

        private void CommonStartup(TextOutputFromGame textOutput, PlayerDiedDelegate playerDiedDelegate, RangedAttackAgainstPlayer rangedAttack)
        {
            m_instance = this;
            m_playerDied += playerDiedDelegate;
            m_textOutput += textOutput;
            m_rangeOnPlayer += rangedAttack;

            // Needs to happen before anything that could create a weapon
            ItemFactory = new ItemFactory();
            MonsterFactory = new MonsterFactory();
            MapObjectFactory = new MapObjectFactory();

            m_saveLoad = new SaveLoadCore();
            
            m_timingEngine = new CoreTimingEngine();

            m_dungeon = new Dictionary<int, Map>();

            StairsMapping.Setup();
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

        internal bool CastSpell(Player caster, Spell spell, Point target)
        {
            return m_physicsEngine.CastSpell(caster, spell, target);
        }

        internal List<Point> SpellCastDrawablePoints(Spell spell, Point target)
        {
            return m_physicsEngine.SpellCastDrawablePoints(spell, target);
        }

        internal bool IsValidTargetForSpell(Spell spell, Point target)
        {
            return m_physicsEngine.IsValidTargetForSpell(spell, target);
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

        internal void RangedAttackOnPlayer(List<Point> rangedPath)
        {
            m_rangeOnPlayer(rangedPath);
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

        internal bool SwapPrimarySecondaryWeapons(Character character, bool canSwapToNothing)
        {
            // If we're swapping to nothing, stop.
            if (character.SecondaryWeapon.GetType() == typeof(MeleeWeapon) && !canSwapToNothing)
                return false;

            IWeapon mainWeapon = character.UnequipWeapon();
            IWeapon secondaryWeapon = character.UnequipSecondaryWeapon();
            
            if (secondaryWeapon != null)
                character.EquipWeapon(secondaryWeapon);
            
            if (mainWeapon != null)
                character.EquipSecondaryWeapon(mainWeapon);

            m_timingEngine.ActorDidMinorAction(character);
            return true;
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
                case "Equip as Secondary":
                {
                    // This probally should live in the player code
                    m_player.RemoveItem(item as Item);
                    Item oldWeapon = m_player.EquipSecondaryWeapon(item as IWeapon) as Item;
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
                case "Unequip as Secondary":
                {
                    Item oldWeapon = m_player.UnequipSecondaryWeapon() as Item;
                    if (oldWeapon != null)
                        m_player.TakeItem(oldWeapon);
                    didSomething = true;
                    break;
                }
                case "Drink":
                case "Read":
                {
                    didSomething = m_physicsEngine.UseItemWithEffect(item as IItemWithEffects);
                    break;
                }
                case "Zap":
                {
                    didSomething = m_physicsEngine.PlayerZapWand(item as Wand);
                    break;
                }
            }
            if (didSomething)
                m_timingEngine.ActorDidAction(m_player);
            return didSomething;
        }
    }
}
