using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Interfaces;

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

        internal DefaultKeystrokeHandler DefaultHandler
        {
            get
            {
                return m_handlers.Values.OfType<DefaultKeystrokeHandler>().First();
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
                namedKey = new NamedKey() { Character = (char)key.Character,
                                            Code = TCODKeyCode.Char,
                                            ControlPressed = (key.LeftControl | key.RightControl),
                                            ShiftPressed = key.Shift,
                                            AltPressed = (key.LeftAlt | key.RightAlt) };
            else
                namedKey = new NamedKey() { Character = (char)0,
                                            Code = key.KeyCode,
                                            ControlPressed = (key.LeftControl | key.RightControl),
                                            ShiftPressed =  key.Shift,
                                            AltPressed = (key.LeftAlt | key.RightAlt) };

            m_currentHandler.HandleKeystroke(namedKey);
        }
    }
}
