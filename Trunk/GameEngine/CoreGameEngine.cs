﻿using System;
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
        private const int MapWidth = 40;
        private const int MapHeight = 5;
        private Player m_player;
        private Map m_map;
        private SaveLoadCore m_saveLoad;
        private PathfindingMap m_pathFinding;
        private PhysicsEngine m_physicsEngine;

        public CoreGameEngine()
        {
            m_player = new Player(1, 1);
            m_map = new Map(MapWidth, MapHeight);
            m_saveLoad = new SaveLoadCore();

            m_physicsEngine = new PhysicsEngine(m_player, m_map);
            m_pathFinding = new PathfindingMap(m_physicsEngine.FOVManager);

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
            m_saveLoad.SaveGame(this);
        }

        internal void Load()
        {
            m_saveLoad.LoadGame(this);
            m_physicsEngine.GameLoaded(m_player, m_map);
        }

        internal bool IsMovablePoint(Map map, Player player, Point p)
        {
            return m_physicsEngine.IsMovablePoint(map, player, p);
        }

        internal bool IsPathablePoint(Map map, Point p)
        {
            return m_physicsEngine.IsPathablePoint(map, p);
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

        internal IList<Point> PathToPoint(Point source, Point dest)
        {
            return m_pathFinding.Travel(source, dest);
        }
    }
}