using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine;
using Magecrawl.GameUI;
using Magecrawl.Utilities;

namespace MageCrawl
{
    internal sealed class GameInstance : IDisposable
    {
        private bool m_isQuitting = false;
        private bool m_isFovMode = false;
        private RootConsole m_console;
        private CoreGameEngine m_engine;
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
            m_engine = new CoreGameEngine();
            m_keystroke = new KeystrokeManager(m_engine);

            do
            {
                KeystrokeResult keyResult = m_keystroke.HandleKeyStroke();
                if (keyResult == KeystrokeResult.Quit)
                    m_isQuitting = true;
                else if (keyResult == KeystrokeResult.FovOn)
                    m_isFovMode = !m_isFovMode;

                m_console.Clear();
                MapDrawer.DrawMap(m_engine.Player, m_engine.Map, m_console);
                if (m_isFovMode)
                    MapDrawer.DrawFov(m_console, m_engine);

                m_console.Flush();
            }
            while (!m_console.IsWindowClosed() && !m_isQuitting);
        }
    }
}
