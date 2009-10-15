using System;
using System.Collections.Generic;
using GameEngine;
using GameUI;
using libtcodWrapper;

namespace MageCrawl
{
    internal sealed class GameInstance
    {
        private bool m_isQuitting = false;
        private RootConsole m_console;
        private CoreGameEngine m_engine;

        internal GameInstance()
        {
        }

        internal void Go()
        {
            m_console = UIHelper.SetupUI();
            m_engine = new CoreGameEngine();

            do
            {
                m_console.Clear();
                HandleKeyStroke();
                Point playerPosition = m_engine.Player.Position;
                m_console.PutChar(playerPosition.X, playerPosition.Y, '@');
                m_console.Flush();
            }
            while (!m_console.IsWindowClosed() && !m_isQuitting);
        }
        
        private void HandleKeyStroke()
        {
            KeyPress key = libtcodWrapper.Keyboard.CheckForKeypress(libtcodWrapper.KeyPressType.Pressed);
            switch (key.KeyCode)
            {
                case KeyCode.TCODK_CHAR:
                {
                    switch ((char)key.Character)
                    {
                        case 'q':
                            m_isQuitting = true;
                            break;
                    }
                    break;
                }
                case KeyCode.TCODK_UP:
                    m_engine.MovePlayer(MovementDirection.North);
                    break;
                case KeyCode.TCODK_DOWN:
                    m_engine.MovePlayer(MovementDirection.South);
                    break;
                case KeyCode.TCODK_LEFT:
                    m_engine.MovePlayer(MovementDirection.West);
                    break;
                case KeyCode.TCODK_RIGHT:
                    m_engine.MovePlayer(MovementDirection.East);
                    break;
                case KeyCode.TCODK_HOME:
                    m_engine.MovePlayer(MovementDirection.Northwest);
                    break;
                case KeyCode.TCODK_PAGEUP:
                    m_engine.MovePlayer(MovementDirection.Northeast);
                    break;
                case KeyCode.TCODK_PAGEDOWN:
                    m_engine.MovePlayer(MovementDirection.Southwest);
                    break;
                case KeyCode.TCODK_END:
                    m_engine.MovePlayer(MovementDirection.Southeast);
                    break;
            }
        }
    }
}
