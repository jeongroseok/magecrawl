using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using libtcod;
using Magecrawl.Exceptions;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.GameUI.Utilities;
using Magecrawl.Interfaces;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

namespace Magecrawl
{
    internal sealed class GameInstance : IDisposable
    {
        internal bool IsQuitting { get; set; }
        internal TextBox TextBox { get; set; }
        private TCODConsole m_console;

        [Import]
        private IGameEngine m_engine;

        // Apparently, msvc is ignorant of MEF and throws a warning on MEFed things that are private/protected. Disable warning.
        #pragma warning disable 649
        [ImportMany]
        private Lazy<IKeystrokeHandler, IDictionary<string, object>>[] m_keystrokeImports;
        #pragma warning restore 649

        private CompositionContainer m_container;

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

            Compose();

            TextBox = new TextBox();
            m_painters = new PaintingCoordinator();

            // Most of the time while debugging, we don't want to save on window close
            ShouldSaveOnClose = !Preferences.Instance.DebuggingMode;
        }

        public void Compose()
        {
            using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Loading Game..."))
            {
                // We need to add the ExecutingAssembly since it is an exe, it doesn't get added by the DirectoryCatalog
                m_container = new CompositionContainer(
                    new AggregateCatalog(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()),
                    new DirectoryCatalog(".")));
                m_container.ComposeParts(this);
            }
        }

        public void Dispose()
        {
            m_container.Dispose();
            m_engine = null;
            if (m_painters != null)
                m_painters.Dispose();
            m_painters = null;
        }

        private void ShowWelcomeMessage(bool firstTime)
        {
            if (firstTime)
                TextBox.AddText(string.Format("If this is your first time, press '{0}' for help.", m_keystroke.DefaultHandler.GetCommandKey("Help")));
            if (m_engine.Player.SkillPoints > 0)
                TextBox.AddText(string.Format("You have skill points to spent. Press '{0}' to open the skill tree.", m_keystroke.DefaultHandler.GetCommandKey("ShowSkillTree")));
            if (firstTime)
                TextBox.AddText("Welcome To Magecrawl.");
            else
                TextBox.AddText("Welcome Back To Magecrawl.");
        }

        internal void StartGameFromFile(string playerName)
        {
            SetupEngineOutputDelegate();
            try
            {
                LoadFromFile(playerName);
            }
            catch (FileNotFoundException)
            {
                StartNewGame(playerName);
            }
            Go(playerName);
        }

        internal void StartNewGame(string playerName)
        {
            SetupEngineOutputDelegate();

            NewPlayerOptionsWindow window = new NewPlayerOptionsWindow();
            string selectedOption = window.Run();

            // If they closed the window on option screen, quit early.
            if (TCODConsole.isWindowClosed() || selectedOption == null)
                return;

            GenerateWorld(playerName, selectedOption);

            Go(playerName);
        }
        
        private void Go(string playerName)
        {
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
                    // Depending on how many levels we are burried in TargetInvocationException
                    // We might have win/died burried. This loop will dig for them.
                    bool handledException = false;
                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        if (inner is PlayerDiedException)
                        {
                            HandleException(true);
                            handledException = true;
                        }
                        else if (inner is PlayerWonException)
                        {
                            HandleException(false);
                            handledException = true;
                        }
                        inner = inner.InnerException;
                    }
                    if (!handledException)
                        throw;
                }
            }
            while (!TCODConsole.isWindowClosed() && !IsQuitting);
            
            // User closed the window, save and bail.
            if (TCODConsole.isWindowClosed() && !IsQuitting && ShouldSaveOnClose)
            {
                m_engine.Save();
            }
        }

        private void GenerateWorld(string playerName, string startingBackground)
        {
            using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Generating World..."))
            {
                m_engine.CreateNewWorld(playerName, startingBackground);
            }

            SetupKeyboardHandlers();  // Requires game engine.
            if (!Preferences.Instance.DebuggingMode)
                SetHandlerName("Welcome");
            else
                SetHandlerName("Default");

            ShowWelcomeMessage(true);
        }

        private void LoadFromFile(string playerName)
        {
            string saveFilePath = playerName + ".sav";

            using (LoadingScreen loadingScreen = new LoadingScreen(m_console, "Loading..."))
            {
                m_engine.LoadSaveFile(saveFilePath);
            }

            SetupKeyboardHandlers();  // Requires game engine.
            SetHandlerName("Default");

            ShowWelcomeMessage(false);
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
            
            ISpell attackingSpell = attackingMethod as ISpell;
            if (attackingSpell != null)
                colorOfBolt = SpellGraphicsInfo.GetColorOfSpellFromSchool(attackingSpell.School);

            IItem attackingItem = attackingMethod as IItem;
            if (attackingItem != null)
                colorOfBolt = SpellGraphicsInfo.GetColorOfSpellFromSchool(m_engine.GameState.GetSpellSchoolForItem(attackingItem));

            switch (type)
            {
                case ShowRangedAttackType.RangedBolt:
                {
                    m_painters.HandleRequest(new ShowRangedBolt((List<Point>)data, colorOfBolt, !targetAtEndPoint, 1));
                    m_painters.DrawAnimationSynchronous(m_console);
                    break;
                }
                case ShowRangedAttackType.RangedBlast:
                {
                    m_painters.HandleRequest(new ShowRangedBolt((List<Point>)data, colorOfBolt, true, 3));
                    m_painters.DrawAnimationSynchronous(m_console);
                    break;
                }
                case ShowRangedAttackType.Cone:
                {
                    m_painters.HandleRequest(new ShowConeBlast((List<Point>)data, colorOfBolt));
                    m_painters.DrawAnimationSynchronous(m_console);
                    break;
                }
                case ShowRangedAttackType.RangedExplodingPoint:
                {
                    var animationData = (Pair<List<Point>, List<List<Point>>>)data;
                    m_painters.HandleRequest(new ShowExploadingPoint(animationData.First, animationData.Second, colorOfBolt));
                    m_painters.DrawAnimationSynchronous(m_console);
                    break;
                }
                case ShowRangedAttackType.Stream:
                {
                    List<Point> coveredSquares = (List<Point>)data;
                    Dictionary<Point, bool> occupiedSquares = new Dictionary<Point, bool>(PointEqualityComparer.Instance);
                    
                    foreach (Point p in coveredSquares)
                    {
                        bool occupied = m_engine.Map.Monsters.Any(x => x.Position == p) || m_engine.Map.MapObjects.Any(x => x.Position == p);
                        occupiedSquares.Add(p, occupied);
                    }
                    m_painters.HandleRequest(new ShowStream(coveredSquares, colorOfBolt, occupiedSquares));
                    m_painters.DrawAnimationSynchronous(m_console);
                    break;
                }
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
                if (TCODConsole.waitForKeypress(true).Character == 'q')
                    break;
            }
        }

        private void SetupKeyboardHandlers()
        {
            m_keystroke = new KeystrokeManager(m_engine);

            foreach (var keystrokeHandlerImport in m_keystrokeImports)
            {
                bool requiresAllActionsMapped = Boolean.Parse((string)keystrokeHandlerImport.Metadata["RequireAllActionsMapped"]);
                string handlerName = (string)keystrokeHandlerImport.Metadata["HandlerName"];
                BaseKeystrokeHandler handler = (BaseKeystrokeHandler)keystrokeHandlerImport.Value;
                handler.Init(m_engine, this, requiresAllActionsMapped);
                m_keystroke.Handlers.Add(handlerName, handler);
            }            

            if (BaseKeystrokeHandler.ErrorsParsingKeymapFiles != "")
            {
                TextBox.AddText("");
                TextBox.AddText(BaseKeystrokeHandler.ErrorsParsingKeymapFiles);
                TextBox.AddText("");
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
            SetHandlerName("Default");
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
