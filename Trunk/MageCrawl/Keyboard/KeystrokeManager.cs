using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using libtcod;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    public sealed class KeystrokeManager
    {
        private Dictionary<string, IKeystrokeHandler> m_handlers = new Dictionary<string, IKeystrokeHandler>();
        private IKeystrokeHandler m_currentHandler;
        private string m_currentHandlerName;

        internal KeystrokeManager(IGameEngine engine)
        {
        }

        public Dictionary<string, IKeystrokeHandler> Handlers
        {
            get
            {
                return m_handlers;
            }
        }

        public IKeystrokeHandler CurrentHandler
        {
            get
            {
                return m_currentHandler;
            }
        }

        public string CurrentHandlerName
        {
            get
            {
                return m_currentHandlerName;
            }
            set
            {
                m_currentHandlerName = value;
                m_currentHandler = m_handlers[value];
            }
        }

        public void HandleKeyStroke()
        {
            TCODKey key = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);

            NamedKey namedKey;

            // Some keys, like backspace, space, and tab have both a character and a code. We only stick one in the dictionary
            // Strip the other one out.
            if (key.KeyCode == TCODKeyCode.Char)
                namedKey = new NamedKey() { Character = (char)key.Character, Code = TCODKeyCode.Char, ControlPressed = (key.LeftControl | key.RightControl) };
            else
                namedKey = new NamedKey() { Character = (char)0, Code = key.KeyCode, ControlPressed = (key.LeftControl | key.RightControl) };

            m_currentHandler.HandleKeystroke(namedKey);
        }
    }
}
