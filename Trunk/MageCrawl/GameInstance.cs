using System;
using System.Collections.Generic;
using Magecrawl.GameEngine;
using Magecrawl.GameUI;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace MageCrawl
{
    internal sealed class GameInstance
    {
        private bool m_isQuitting = false;
        private RootConsole m_console;
        private CoreGameEngine m_engine;
        private KeystrokeManager m_keystroke;

        internal GameInstance()
        {
        }

        internal void Go()
        {
            m_console = UIHelper.SetupUI();
            m_engine = new CoreGameEngine();
            m_keystroke = new KeystrokeManager(m_engine);

            do
            {
                KeystrokeResult keyResult = m_keystroke.HandleKeyStroke();
                if (keyResult == KeystrokeResult.Quit)
                    m_isQuitting = true;

                m_console.Clear();
                MapDrawer.DrawMap(m_engine.Player, m_engine.Map, m_console);
                m_console.Flush();
            }
            while (!m_console.IsWindowClosed() && !m_isQuitting);
        }
    }
}
