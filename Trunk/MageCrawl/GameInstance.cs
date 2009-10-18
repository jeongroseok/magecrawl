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
        private TextBox m_textBox;

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
            m_textBox = new TextBox();
            m_engine = new PublicGameEngine(new PublicGameEngine.TextOutputFromGame(m_textBox.TextInputFromEngineDelegate));
            m_keystroke = new KeystrokeManager(m_engine);
            m_keystroke.LoadKeyMappings();

            do
            {
                HandleKeyboard();

                m_console.Clear();
                MapDrawer.DrawMap(m_engine.Player, m_engine.Map, m_console);
                m_textBox.Draw(m_console);
                if (m_pathableMode)
                    MapDrawer.DrawPathable(m_console, m_engine);

                m_console.Flush();
            }
            while (!m_console.IsWindowClosed() && !m_isQuitting);
        }

        private void HandleKeyboard()
        {
            KeystrokeResult keyResult = m_keystroke.HandleKeyStroke();
            switch (keyResult)
            { 
                case KeystrokeResult.Quit:
                    m_isQuitting = true;
                    break;
                case KeystrokeResult.PathableOn:
                    m_pathableMode = !m_pathableMode;
                    break;
                case KeystrokeResult.TextBoxClear:
                    m_textBox.Clear();
                    break;
                case KeystrokeResult.TextBoxDown:
                    m_textBox.textBoxScrollDown();
                    break;
                case KeystrokeResult.TextBoxUp:
                    m_textBox.textBoxScrollUp();
                    break;
            }    
        }
    }
}
