using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.Keyboard
{
    class OperateKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public OperateKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        private void North()
        {
            m_engine.Operate(Direction.North);
            Escape();
        }

        private void South()
        {
            m_engine.Operate(Direction.South);
            Escape();
        }

        private void East()
        {
            m_engine.Operate(Direction.East);
            Escape();
        }

        private void West()
        {
            m_engine.Operate(Direction.West);
            Escape();
        }

        private void Northeast()
        {
            m_engine.Operate(Direction.Northeast);
            Escape();
        }

        private void Northwest()
        {
            m_engine.Operate(Direction.Northwest);
            Escape();
        }

        private void Southeast()
        {
            m_engine.Operate(Direction.Southeast);
            Escape();
        }

        private void Southwest()
        {
            m_engine.Operate(Direction.Southwest);
            Escape();
        }

        private void Escape()
        {
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }
    }
}
