using System;
using libtcodWrapper;
using Magecrawl.Exceptions;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Dialogs;
using Magecrawl.Keyboard.Inventory;
using Magecrawl.Keyboard.Magic;
using Magecrawl.Utilities;

namespace Magecrawl
{
    internal sealed class GameInstance : IDisposable
    {
        internal bool IsQuitting { get; set; }
        internal TextBox TextBox { get; set; }
        private RootConsole m_console;
        private IGameEngine m_engine;
        private KeystrokeManager m_keystroke;
        private CharacterInfo m_charInfo;
        private PaintingCoordinator m_painters;

        public bool ShouldSaveOnClose
        {
            get;
            set;
        }

        internal GameInstance()
        {
            TextBox = new TextBox();
            m_charInfo = new CharacterInfo();
            m_painters = new PaintingCoordinator();

            // Most of the time while debugging, we don't want to save on window close
            ShouldSaveOnClose = !Preferences.Instance.DebuggingMode;
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

            TextOutputFromGame outputDelegate = new TextOutputFromGame(TextBox.TextInputFromEngineDelegate);
            PlayerDiedDelegate diedDelegate = new PlayerDiedDelegate(HandlePlayerDied);

            if (System.IO.File.Exists("Donblas.sav"))
            {
                using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Loading..."))
                    m_engine = new PublicGameEngine(outputDelegate, diedDelegate, "Donblas.sav");

                SetupKeyboardHandlers();  // Requires game engine.
                SetHandlerName("Default");
            }
            else
            {
                using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Generating World..."))
                    m_engine = new PublicGameEngine(outputDelegate, diedDelegate);

                SetupKeyboardHandlers();  // Requires game engine.
                if (!Preferences.Instance.DebuggingMode)
                    SetHandlerName("Welcome");
                else
                    SetHandlerName("Default");
            }

            // First update before event loop so we have a map to display
            m_painters.UpdateFromNewData(m_engine);

            do
            {
                try
                {
                    HandleKeyboard();
                    m_console.Clear();
                    TextBox.Draw(m_console);
                    m_charInfo.Draw(m_console, m_engine, m_engine.Player);
                    m_painters.DrawNewFrame(m_console);
                    m_console.Flush();
                }
                catch (PlayerDiedException)
                {
                    HandleException(true);
                }
                catch (PlayerWonException)
                {
                    HandleException(false);
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    if (e.InnerException is PlayerDiedException)
                    {
                        HandleException(true);
                    }
                    else if (e.InnerException is PlayerWonException)
                    {
                        HandleException(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (!m_console.IsWindowClosed() && !IsQuitting);
            
            // User closed the window, save and bail.
            if (m_console.IsWindowClosed() && !IsQuitting && ShouldSaveOnClose)
            {
                m_engine.Save();
            }
        }

        private void HandleException(bool death)
        {
            HandleGameOver(death ? "Player has died." : "Player has won.");
            IsQuitting = true;
        }

        private void HandleGameOver(string textToDisplay)
        {
            // Put death information out here.
            m_painters.UpdateFromNewData(m_engine);
            SendPaintersRequest(new DisableAllOverlays());
            m_charInfo.Draw(m_console, m_engine, m_engine.Player);
            m_painters.DrawNewFrame(m_console);
            TextBox.AddText(textToDisplay);
            TextBox.AddText("Press 'q' to exit.");
            TextBox.Draw(m_console);
            m_console.Flush();

            while (!m_console.IsWindowClosed())
            {
                if (libtcodWrapper.Keyboard.CheckForKeypress(KeyPressType.Pressed).Character == 'q')
                    break;
            }
        }

        private void SetupKeyboardHandlers()
        {
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
            defaultHandler.LoadKeyMappings(true);
            m_keystroke.Handlers.Add("Default", defaultHandler);

            TargettingKeystrokeHandler attackHandler = new TargettingKeystrokeHandler(m_engine, this);
            attackHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Target", attackHandler);

            ViewmodeKeystrokeHandler viewmodeHandler = new ViewmodeKeystrokeHandler(m_engine, this);
            viewmodeHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Viewmode", viewmodeHandler);

            MagicListKeyboardHandler magicList = new MagicListKeyboardHandler(m_engine, this);
            magicList.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("SpellList", magicList);

            InventoryScreenKeyboardHandler inventoryHandler = new InventoryScreenKeyboardHandler(m_engine, this);
            inventoryHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Inventory", inventoryHandler);

            EquipmentScreenKeyboardHandler equipmentHandler = new EquipmentScreenKeyboardHandler(m_engine, this);
            equipmentHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Equipment", equipmentHandler);

            InventoryItemKeyboardHandler inventoryItemHandler = new InventoryItemKeyboardHandler(m_engine, this);
            inventoryItemHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("InventoryItem", inventoryItemHandler);

            WelcomeKeyboardHandler welcomeHandler = new WelcomeKeyboardHandler(m_engine, this);
            welcomeHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Welcome", welcomeHandler);

            SaveGameKeyboardHandler saveGameHandler = new SaveGameKeyboardHandler(m_engine, this);
            saveGameHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("SaveGame", saveGameHandler);

            QuitGameKeyboardHandler quitGameHandler = new QuitGameKeyboardHandler(m_engine, this);
            quitGameHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("QuitGame", quitGameHandler);

            MapEffectsKeystrokeHandler effectHandler = new MapEffectsKeystrokeHandler(m_engine, this);
            effectHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Effects", effectHandler);

            HelpKeyboardHandler helpHandler = new HelpKeyboardHandler(m_engine, this);
            helpHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("Help", helpHandler);

            OneButtonDialogKeyboardHandler oneButtonHandler = new OneButtonDialogKeyboardHandler(m_engine, this);
            oneButtonHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("OneButtonDialog", oneButtonHandler);
        }

        internal void SetHandlerName(string s)
        {
            SetHandlerName(s, null, null, null, null);
        }

        internal void SetHandlerName(string s, object objOne)
        {
            SetHandlerName(s, objOne, null, null, null);
        }

        internal void SetHandlerName(string s, object objOne, object objTwo)
        {
            SetHandlerName(s, objOne, objTwo, null, null);
        }

        internal void SetHandlerName(string s, object objOne, object objTwo, object objThree)
        {
            SetHandlerName(s, objOne, objTwo, objThree, null);
        }

        internal void SetHandlerName(string s, object objOne, object objTwo, object objThree, object objFour)
        {
            m_keystroke.CurrentHandlerName = s;
            m_keystroke.CurrentHandler.NowPrimaried(objOne, objTwo, objThree, objFour);
        }

        internal void ResetHandlerName()
        {
            m_keystroke.CurrentHandlerName = "Default";
        }

        private void HandlePlayerDied()
        {
            // So we want player dead to hault pretty much everything. While an exception is 
            // probally not the 'best' solution, since we're in a callback from the engine, made from a request from HandleKeyboard
            // it's easy.
            throw new PlayerDiedException();
        }

        internal void SendPaintersRequest(RequestBase request)
        {
            m_painters.HandleRequest(request);
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
