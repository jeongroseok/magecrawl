using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

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
        private static PlayerDiedDelegate m_playerDied;

        public CoreGameEngine(PlayerDiedDelegate diedDelegate)
        {
            m_playerDied = diedDelegate;

            m_player = new Player(1, 1);
            m_map = new Map(50, 50);
            m_saveLoad = new SaveLoadCore();

            m_physicsEngine = new PhysicsEngine(m_player, m_map);
            m_pathFinding = new PathfindingMap(m_player, m_map);

            // If the player isn't the first actor, let others go. See archtecture note in PublicGameEngine.
            m_physicsEngine.AfterPlayerAction(this);

            PublicGameEngine.SendTextOutput("Welcome To Magecrawl");
        }

        public void Dispose()
        {
            if (m_physicsEngine != null)
                m_physicsEngine.Dispose();
            m_physicsEngine = null;

            if (m_pathFinding != null)
                m_pathFinding.Dispose();
            m_pathFinding = null;
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
            PublicGameEngine.SendTextOutput("Saving Game.");
            m_saveLoad.SaveGame(this);
        }

        internal void Load()
        {
            PublicGameEngine.SendTextOutput("Loading Game.");
            m_saveLoad.LoadGame(this);
            m_pathFinding.Dispose();
            m_pathFinding = new PathfindingMap(m_player, m_map);
            m_physicsEngine.GameLoaded(m_player, m_map);
            PublicGameEngine.SendTextOutput("Game Loaded.");
        }

        internal bool IsMovablePoint(Map map, Player player, Point p)
        {
            return m_physicsEngine.IsMovablePoint(map, player, p);
        }

        internal bool Move(Character c, Direction direction)
        {
            return m_physicsEngine.Move(c, direction);
        }

        internal bool Attack(Character attacker, Direction direction)
        {
            return m_physicsEngine.Attack(attacker, direction);
        }

        public bool Operate(Character characterOperating, Direction direction)
        {
            return m_physicsEngine.Operate(characterOperating, direction);
        }

        internal bool Wait(Character c)
        {
            return m_physicsEngine.Wait(c);
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

        public bool[,] PlayerMoveableToEveryPoint()
        {
            return m_physicsEngine.CalculateMoveablePointGrid(m_map, m_player);
        }

        internal static void PlayerDied()
        {
            m_playerDied();
        }

        internal FOVManager FOVManager
        {
            get
            {
                return m_physicsEngine.FOVManager;
            }
        }
    }
}
