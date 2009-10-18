using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine;

namespace MageCrawl
{
    internal enum KeystrokeResult
    {
        None,
        InOperate,
        PathableOn,
        Quit
    }

    internal sealed class KeystrokeManager
    {
        private CoreGameEngine m_engine;
        private bool m_inOperate;

        internal KeystrokeManager(CoreGameEngine engine)
        {
            m_engine = engine;
            m_inOperate = false;
        }

        public KeystrokeResult HandleKeyStroke()
        {
            KeyPress key = libtcodWrapper.Keyboard.CheckForKeypress(libtcodWrapper.KeyPressType.Pressed);

            bool handled = HandleMovementArrow(key);

            if (handled)
            {
                // If we didn't handle with movement, we're not in an operation chord
                m_inOperate = false;
            }
            else
            {
                switch (key.KeyCode)
                {
                    case KeyCode.TCODK_CHAR:
                        {
                            switch ((char)key.Character)
                            {
                                case 'q':
                                    return KeystrokeResult.Quit;
                                case 'o':
                                    m_inOperate = true;
                                    return KeystrokeResult.InOperate;
                                case 'S':
                                    m_engine.Save();
                                    return KeystrokeResult.None;
                                case 'L':
                                {
                                    try
                                    {
                                        m_engine.Load();
                                        return KeystrokeResult.None;
                                    }
                                    catch (System.IO.FileNotFoundException)
                                    {
                                        // TODO: Inform user somehow.
                                        return KeystrokeResult.None;
                                    }
                                }
                                case 'f':
                                    return KeystrokeResult.PathableOn;
                                case '.':
                                    m_engine.PlayerWait();
                                    return KeystrokeResult.None;
                            }
                            break;
                        }
                }
            }
            return KeystrokeResult.None;
        }

        private bool HandleMovementArrow(KeyPress key)
        {
            Direction direction;
            switch (key.KeyCode)
            {
                case KeyCode.TCODK_UP:
                    direction = Direction.North;
                    break;
                case KeyCode.TCODK_DOWN:
                    direction = Direction.South;
                    break;
                case KeyCode.TCODK_LEFT:
                    direction = Direction.West;
                    break;
                case KeyCode.TCODK_RIGHT:
                    direction = Direction.East;
                    break;
                case KeyCode.TCODK_HOME:
                    direction = Direction.Northwest;
                    break;
                case KeyCode.TCODK_PAGEUP:
                    direction = Direction.Northeast;
                    break;
                case KeyCode.TCODK_PAGEDOWN:
                    direction = Direction.Southeast;
                    break;
                case KeyCode.TCODK_END:
                    direction = Direction.Southwest;
                    break;
                default:
                    direction = Direction.None;
                    break;
            }
            if (direction != Direction.None)
            {
                if (m_inOperate)
                    m_engine.Operate(direction);
                else
                    m_engine.MovePlayer(direction);
                return true;
            }
            return false;
        }
    }
}
