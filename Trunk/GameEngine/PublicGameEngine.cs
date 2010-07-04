using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Effects;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.Skills;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    // So in the current archtecture, each public method should do the action requested,
    // and _then_ call the CoreTimingEngine somehow to let others have their time slice before returning
    // This is very synchronous, but easy to do.
    [Export (typeof(IGameEngine))]
    public class PublicGameEngine : IGameEngine
    {
        private CoreGameEngine m_engine;
        private DebugEngine m_debugEngine;
        private PlayerActionEngine m_actionEngine;
        private GameStateInterface m_gameState;
        private TargettingUtils m_targetting;

        public event TextOutputFromGame TextOutputEvent;
        public event PlayerDied PlayerDiedEvent;
        public event RangedAttack RangedAttackEvent;

        public PublicGameEngine()
        {
            // This is a singleton accessable from anyone in GameEngine, but stash a copy since we use it alot
            m_engine = new CoreGameEngine();
            m_engine.TextOutputEvent += new TextOutputFromGame(s => TextOutputEvent(s));
            m_engine.PlayerDiedEvent += new PlayerDied(() => PlayerDiedEvent());
            m_engine.RangedAttackEvent += new RangedAttack((a, type, d, targetAtEnd) => RangedAttackEvent(a, type, d, targetAtEnd));
            m_debugEngine = new DebugEngine(m_engine);
            m_actionEngine = new PlayerActionEngine(m_engine);
            m_gameState = new GameStateInterface(m_engine);
            m_targetting = new TargettingUtils(m_engine);
        }

        public void CreateNewWorld(string playerName)
        {
            m_engine.CreateNewWorld(playerName);
        }

        public void LoadSaveFile(string saveGameName)
        {
            m_engine.LoadSaveFile(saveGameName);
        }

        public void Dispose()
        {
            if (m_engine != null)
                m_engine.Dispose();
            m_engine = null;
        }

        public Point TargetSelection
        {
            get;
            set;
        }

        public bool SelectingTarget
        {
            get;
            set;
        }

        public IPlayer Player
        {
            get
            {
                return m_engine.Player;
            }
        }

        public IMap Map
        {
            get
            {
                return m_engine.Map;
            }
        }

        public int CurrentLevel
        {
            get
            {
                return m_engine.CurrentLevel;
            }
        }

        public int TurnCount 
        {
            get
            {
                return m_engine.TurnCount;
            }
        }
      
        public void Save()
        {
            m_engine.Save();
        }

        public IGameState GameState
        {
            get
            {
                return m_gameState;
            }
        }
    
        public IDebugger Debugger
        {
            get
            {
                return m_debugEngine;
            }
        }

        public IEngineActions Actions
        {
            get
            {
                return m_actionEngine;
            }
        }

        public ITargettingUtils Targetting
        {
            get
            {
                return m_targetting;
            }
        }
    }
}