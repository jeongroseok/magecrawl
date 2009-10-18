using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine
{
    // So in the current archtecture, each public method should do the action requested,
    // and _then_ call the CoreTimingEngine somehow to let others have their time slice before returning
    // This is very synchronous, but easy to do.
    public class PublicGameEngine : IGameEngine
    {
        private CoreGameEngine m_engine;

        public PublicGameEngine()
        {
            m_engine = new CoreGameEngine();
        }

        public void Dispose()
        {
            if (m_engine != null)
                m_engine.Dispose();
            m_engine = null;
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

        public bool MovePlayer(Direction direction)
        {
            bool didAnything = m_engine.Move(m_engine.Player, direction);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool Operate(Direction direction)
        {
            bool didAnything = m_engine.Operate(m_engine.Player, direction);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool PlayerWait()
        {
            bool didAnything = m_engine.Wait(m_engine.Player);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool PlayerAttack(Direction direction)
        {
            bool didAnything = m_engine.Attack(m_engine.Player, direction);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public void Save()
        {
            m_engine.Save();
        }

        public void Load()
        {
            m_engine.Load();
        }

        public IList<Magecrawl.Utilities.Point> PlayerPathToPoint(Magecrawl.Utilities.Point dest)
        {
            return m_engine.PathToPoint(m_engine.Player.Position, dest);
        }
    }
}
