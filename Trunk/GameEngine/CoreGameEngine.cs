using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine
{
    // This class mostly coordinates between a bunch of helper classes to provide what PublicGameEngine needs.
    internal sealed class CoreGameEngine : IDisposable
    {
        private Player m_player;
        private Map m_map;
        private SaveLoadCore m_saveLoad;
        private PathfindingMap m_pathFinding;
        private PhysicsEngine m_physicsEngine;

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
            m_instance = this;
            m_playerDied += diedDelegate;
            m_textOutput += textOutput;

            m_player = new Player(1, 1);
            m_map = new Map(50, 50);
            m_saveLoad = new SaveLoadCore();

            m_physicsEngine = new PhysicsEngine(m_player, m_map);
            m_pathFinding = new PathfindingMap(m_player, m_map);

            // If the player isn't the first actor, let others go. See archtecture note in PublicGameEngine.
            m_physicsEngine.AfterPlayerAction(this);

            SendTextOutput("Welcome To Magecrawl");
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
                return m_map;
            }
        }

        internal void SetWithSaveData(Player p, Map m)
        {
            m_player = p;
            m_map = m;
        }

        internal void Save()
        {
            SendTextOutput("Saving Game.");
            m_saveLoad.SaveGame(this);
        }

        internal void Load()
        {
            SendTextOutput("Loading Game.");
            m_saveLoad.LoadGame(this);
            m_pathFinding.Dispose();
            m_pathFinding = new PathfindingMap(m_player, m_map);
            m_physicsEngine.GameLoaded(m_player, m_map);
            SendTextOutput("Game Loaded.");
        }

        internal bool Move(Character c, Direction direction)
        {
            return m_physicsEngine.Move(c, direction);
        }

        internal bool Attack(Character attacker, Point target)
        {
            return m_physicsEngine.Attack(attacker, target);
        }

        internal bool CastSpell(Character attacker, SpellBase spell)
        {
            m_physicsEngine.CastSpell(this, attacker, spell);
            return true;
        }

        public bool Operate(Character characterOperating, Direction direction)
        {
            return m_physicsEngine.Operate(characterOperating, direction);
        }

        internal bool Wait(Character c)
        {
            return m_physicsEngine.Wait(c);
        }

        internal bool PlayerGetItem()
        {
            return m_physicsEngine.PlayerGetItem();
        }

        // Called by PublicGameEngine after any call to CoreGameEngine which passes time.
        internal void AfterPlayerAction()
        {
            m_physicsEngine.AfterPlayerAction(this);
        }

        internal IList<Point> PathToPoint(Character actor, Point dest, bool canOperate)
        {
            return m_pathFinding.Travel(actor, dest, canOperate, m_physicsEngine);
        }

        // This is used by the Movability Debug View. If you think you need it, you don't. Talk to chris. 
        // Unless your me, this smack yourself and try again.
        public bool[,] PlayerMoveableToEveryPoint()
        {
            return m_physicsEngine.CalculateMoveablePointGrid(m_map, m_player);
        }

        public void FilterNotTargetablePointsFromList(List<WeaponPoint> pointList, Point characterPosition, int visionRange)
        {
            m_physicsEngine.FilterNotTargetablePointsFromList(pointList, characterPosition, visionRange);
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

        internal List<ItemOptions> GetOptionsForInventoryItem(IItem item)
        {
            Item currentItem = item as Item;
            return currentItem.PlayerOptions;
        }

        internal bool PlayerSelectedItemOption(IItem item, string option)
        {
            switch (option)
            {
                case "Drop":
                    return m_physicsEngine.PlayerDropItem(item as Item);
            }
            return false;
        }
    }
}
