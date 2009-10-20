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
        private RootConsole m_console;
        private IGameEngine m_engine;
        private KeystrokeManager m_keystroke;
        private CharacterInfo m_charInfo;
        private TextBox m_textBox;
        private MapDrawer m_mapDrawer;

        internal GameInstance()
        {
            m_textBox = new TextBox();
            m_charInfo = new CharacterInfo();
            m_mapDrawer = new MapDrawer();
        }

        public void Dispose()
        {
            if(m_engine != null)
                m_engine.Dispose();
            m_engine = null;

            if (m_mapDrawer != null)
                m_mapDrawer.Dispose();
            m_mapDrawer = null;
        }
        
        internal void Go()
        {
            m_console = UIHelper.SetupUI();
            PublicGameEngine.TextOutputFromGame outputDelegate = new PublicGameEngine.TextOutputFromGame(m_textBox.TextInputFromEngineDelegate);
            m_engine = new PublicGameEngine(outputDelegate);
            m_keystroke = new KeystrokeManager(m_engine);
            m_keystroke.LoadKeyMappings();

            // First update before event loop so we have a map to display
            m_mapDrawer.UpdateMap(m_engine.Player, m_engine.Map);

            do
            {
                HandleKeyboard();

                m_console.Clear();
                m_mapDrawer.DrawMap(m_console);
                m_textBox.Draw(m_console);
                m_charInfo.Draw(m_console, m_engine.Player);
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
                case KeystrokeResult.PathableOnOff:
                    m_mapDrawer.SwapPathableMode(m_engine);
                    break;
                case KeystrokeResult.TextBoxClear:
                    m_textBox.Clear();
                    break;
                case KeystrokeResult.TextBoxDown:
                    m_textBox.TextBoxScrollDown();
                    break;
                case KeystrokeResult.TextBoxUp:
                    m_textBox.TextBoxScrollUp();
                    break;
                case KeystrokeResult.Action:
                    m_mapDrawer.UpdateMap(m_engine.Player, m_engine.Map);
                    break;
            }    
        }
    }
}
