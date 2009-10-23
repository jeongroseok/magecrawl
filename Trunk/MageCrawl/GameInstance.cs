using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Map;
using Magecrawl.Utilities;
using Magecrawl.Keyboard;

namespace Magecrawl
{
    internal sealed class GameInstance : IDisposable
    {
        internal bool isQuitting { get; set; }
        internal TextBox textBox { get; set; }
        private RootConsole m_console;
        private IGameEngine m_engine;
        private KeystrokeManager m_keystroke;
        private CharacterInfo m_charInfo;
        private MapPaintingCoordinator m_painters;

        internal GameInstance()
        {
            textBox = new TextBox();
            m_charInfo = new CharacterInfo();
            m_painters = new MapPaintingCoordinator();
        }

        public void Dispose()
        {
            if (m_engine != null)
                m_engine.Dispose();
            m_engine = null;
            if (m_painters != null)
                m_painters.Dispose();
            m_painters = null;
        }
        
        internal void Go()
        {
            m_console = UIHelper.SetupUI();
            PublicGameEngine.TextOutputFromGame outputDelegate = new PublicGameEngine.TextOutputFromGame(textBox.TextInputFromEngineDelegate);
            PlayerDiedDelegate diedDelegate = new PlayerDiedDelegate(HandlePlayerDied);
            m_engine = new PublicGameEngine(outputDelegate, diedDelegate);

            /* 
             * BCL: Creating the KeystrokeManager and all IKeystrokeHandlers. In the absence of something like MEF, we can
             * create them all at the top level and initialize them with whatever.
             * 
             * Switching between handlers is as simple as setting the CurrentHandlerName property on the KeystrokeManager, but
             * the difficulty is that KeystrokeManager is internal to MageCrawl. If we want other handlers to live in lower-
             * level assemblies, we need some way to set this property, either through a singleton public KeystrokeManager, or
             * through the GameEngine or GameInstance or whatever.
             */
            m_keystroke = new KeystrokeManager(m_engine);
            DefaultKeystrokeHandler defaultHandler = new DefaultKeystrokeHandler(m_engine, this);
            defaultHandler.LoadKeyMappings();
            m_keystroke.Handlers.Add("Default", defaultHandler);
            m_keystroke.CurrentHandlerName = "Default";

            // First update before event loop so we have a map to display
            m_painters.UpdateFromNewData(m_engine);

            do
            {
                try
                {
                    HandleKeyboard();
                    m_console.Clear();
                    m_painters.DrawNewFrame(m_console);
                    textBox.Draw(m_console);
                    m_charInfo.Draw(m_console, m_engine.Player);
                    m_console.Flush();
                }
                catch (PlayerDiedException)
                {
                    // Put death information out here.
                    textBox.AddText("Player has died.");
                    textBox.AddText("Press Any Key To Quit.");
                    textBox.Draw(m_console);
                    m_console.Flush();
                    libtcodWrapper.Keyboard.WaitForKeyPress(true);
                    isQuitting = true;
                }
            }
            while (!m_console.IsWindowClosed() && !isQuitting);
        }

        private void HandlePlayerDied()
        {
            // So we want player dead to hault pretty much everything. While an exception is 
            // probally not the 'best' solution, since we're in a callback from the engine, made from a request from HandleKeyboard
            // it's easy.
            throw new PlayerDiedException();
        }

        internal void SendPaintersRequest(string s, Object data)
        {
            m_painters.HandleRequest(s, data);
        }

        internal void UpdatePainters()
        {
            m_painters.UpdateFromNewData(m_engine);
        }

        private void HandleKeyboard()
        {
            m_keystroke.HandleKeyStroke();
        }
    }
}
