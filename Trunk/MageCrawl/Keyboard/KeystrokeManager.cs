using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using libtcodWrapper;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.Keyboard;

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
            KeyPress key = libtcodWrapper.Keyboard.CheckForKeypress(libtcodWrapper.KeyPressType.Pressed);

            NamedKey namedKey;

            // Some keys, like backspace, space, and tab have both a character and a code. We only stick one in the dictionary
            // Strip the other one out.
            if (key.KeyCode == KeyCode.TCODK_CHAR)
                namedKey = new NamedKey() { Character = (char)key.Character, Code = KeyCode.TCODK_CHAR };
            else
                namedKey = new NamedKey() { Character = (char)0, Code = key.KeyCode };

            try
            {
                m_currentHandler.HandleKeystroke(namedKey);
            }
            catch (TargetInvocationException e)
            {
                // If the GameEngine throws an exception, pass it up
                throw e.InnerException;
            }
        }
    }
}
