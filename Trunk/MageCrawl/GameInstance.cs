using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using libtcod;
using Magecrawl.Exceptions;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Debug;
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
        private TCODConsole m_console;

        private IGameEngine m_engine;

        private KeystrokeManager m_keystroke;
        private PaintingCoordinator m_painters;

        public bool ShouldSaveOnClose
        {
            get;
            set;
        }

        internal GameInstance()
        {
            m_console = TCODConsole.root;
            using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Loading Game..."))
            {
                m_engine = new PublicGameEngine();
            }

            TextBox = new TextBox();
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
        
        internal void Go(string playerName, bool loadFromFile)
        {
            SetupEngineOutputDelegate();

            if (loadFromFile)
            {
                string saveFilePath = playerName + ".sav";
 
                using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Loading..."))
                {
                    m_engine.LoadSaveFile(saveFilePath);
                }

                SetupKeyboardHandlers();  // Requires game engine.
                SetHandlerName("Default");
            }
            else
            {
                using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Generating World..."))
                {
                    m_engine.CreateNewWorld(playerName);
                }

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
                    DrawFrame();
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
            while (!TCODConsole.isWindowClosed() && !IsQuitting);
            
            // User closed the window, save and bail.
            if (TCODConsole.isWindowClosed() && !IsQuitting && ShouldSaveOnClose)
            {
                m_engine.Save();
            }
        }

        private void SetupEngineOutputDelegate()
        {
            m_engine.PlayerDiedEvent += HandlePlayerDied;
            m_engine.RangedAttackEvent += HandleRangedAttack;
            m_engine.TextOutputEvent += TextBox.AddText;
        }

        public void DrawFrame()
        {
            m_console.clear();
            TextBox.Draw(m_console);
            m_painters.DrawNewFrame(m_console);
            TCODConsole.flush();
        }

        private void HandleRangedAttack(object attackingMethod, ShowRangedAttackType type, object data, bool targetAtEndPoint)
        {
            UpdatePainters();
            ResetHandlerName();
            SendPaintersRequest(new DisableAllOverlays());
            
            TCODColor colorOfBolt = ColorPresets.White;
            if (attackingMethod is ISpell)
                colorOfBolt = SpellGraphicsInfo.GetColorOfSpellFromSchool(((ISpell)attackingMethod).School);
            else if (attackingMethod is IItem)
                colorOfBolt = SpellGraphicsInfo.GetColorOfSpellFromSchool(((IItem)attackingMethod).ItemSchool);

            if (type == ShowRangedAttackType.RangedBoltOrBlast)
            {
                bool drawLastFrame = !targetAtEndPoint;
                int tailLength = 1;

                if (attackingMethod is ISpell)
                {
                    ISpell attackingSpell = (ISpell)attackingMethod;
                    drawLastFrame |= SpellGraphicsInfo.DrawLastFrame(attackingSpell);  // Draw the last frame if we wouldn't otherwise and the spell asks us to.
                    tailLength = SpellGraphicsInfo.GetTailLength(attackingSpell);
                }
                m_painters.HandleRequest(new ShowRangedBolt(null, (List<Point>)data, colorOfBolt, drawLastFrame, tailLength));
                m_painters.DrawAnimationSynchronous(m_console);
            }
            else if (type == ShowRangedAttackType.Cone)
            {
                m_painters.HandleRequest(new ShowConeBlast(null, (List<Point>)data, colorOfBolt));
                m_painters.DrawAnimationSynchronous(m_console);
            }
            else if (type == ShowRangedAttackType.RangedExplodingPoint)
            {
                var animationData = (Pair<List<Point>, List<List<Point>>>)data;
                m_painters.HandleRequest(new ShowExploadingPoint(null, animationData.First, animationData.Second, colorOfBolt));
                m_painters.DrawAnimationSynchronous(m_console);
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
            UpdatePainters();
            SendPaintersRequest(new DisableAllOverlays());
            m_painters.DrawNewFrame(m_console);
            TextBox.AddText(textToDisplay);
            TextBox.AddText("Press 'q' to exit.");
            TextBox.Draw(m_console);
            TCODConsole.flush();

            while (!TCODConsole.isWindowClosed())
            {
                if (TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed).Character == 'q')
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

            ItemOnGroundSelectionKeyboardHandler itemOnGroundHandler = new ItemOnGroundSelectionKeyboardHandler(m_engine, this);
            itemOnGroundHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("ItemOnGroundSelection", itemOnGroundHandler);

            DebugDialogKeyboardHandler debugHandler = new DebugDialogKeyboardHandler(m_engine, this);
            debugHandler.LoadKeyMappings(false);
            m_keystroke.Handlers.Add("DebugMode", debugHandler);

            if (BaseKeystrokeHandler.ErrorsParsingKeymapFiles != string.Empty)
            {
                TextBox.AddText(string.Empty);
                TextBox.AddText(BaseKeystrokeHandler.ErrorsParsingKeymapFiles);
                TextBox.AddText(string.Empty);
            }
        }

        internal void SetHandlerName(string s)
        {
            SetHandlerName(s, null);
        }

        internal void SetHandlerName(string s, object request)
        {
            m_keystroke.CurrentHandlerName = s;
            m_keystroke.CurrentHandler.NowPrimaried(request);
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
