using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI;
using Magecrawl.Utilities;

namespace MageCrawl
{
    internal sealed class GameInstance : IDisposable
    {
        private bool m_isQuitting = false;
        private bool m_pathableMode = false;
        private RootConsole m_console;
        private IGameEngine m_engine;
        private KeystrokeManager m_keystroke;

        internal GameInstance()
        {
        }

        public void Dispose()
        {
            m_engine.Dispose();
        }

        internal void Go()
        {
            m_console = UIHelper.SetupUI();
            m_engine = new PublicGameEngine();
            m_keystroke = new KeystrokeManager(m_engine);
            m_keystroke.LoadKeyMappings();

            do
            {
                KeystrokeResult keyResult = m_keystroke.HandleKeyStroke();
                if (keyResult == KeystrokeResult.Quit)
                    m_isQuitting = true;
                else if (keyResult == KeystrokeResult.PathableOn)
                    m_pathableMode = !m_pathableMode;

                m_console.Clear();
                MapDrawer.DrawMap(m_engine.Player, m_engine.Map, m_console);
                if (m_pathableMode)
                    MapDrawer.DrawPathable(m_console, m_engine);

                m_console.Flush();
            }
            while (!m_console.IsWindowClosed() && !m_isQuitting);
        }
    }
}
