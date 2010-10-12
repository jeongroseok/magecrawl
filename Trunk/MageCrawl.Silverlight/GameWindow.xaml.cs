using System.Windows.Controls;
using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using MageCrawl.Silverlight.KeyboardHandlers;

namespace MageCrawl.Silverlight
{
    public delegate void KeystrokeHandler(MagecrawlKey key, Map map, GameWindow window, IGameEngine engine);

    public partial class GameWindow : UserControl
    {
        private IGameEngine m_engine;
        
        private LostFocusPopup m_focusPopup;
        private bool m_focusPopupEnabled;

        private DefaultKeyboardHandler m_defaultKey;
        private KeystrokeHandler m_currentKeystrokeHandler;

        public GameWindow()
        {
            InitializeComponent();
        }

        public void Setup(IGameEngine engine)
        {
            m_engine = engine;

            m_defaultKey = new DefaultKeyboardHandler(this, m_engine, Map);
            m_currentKeystrokeHandler = m_defaultKey.OnKeyboardDown;

            CharacterInfo.Setup(engine.Player);
            Map.Setup(engine);
            ShowWelcomeMessage(true);

            // Gives SL focus on startup!
            System.Windows.Browser.HtmlPage.Plugin.Focus();

            m_focusPopup = new LostFocusPopup();
            m_focusPopupEnabled = true;
        }

        private void ShowWelcomeMessage(bool firstTime)
        {
            if (firstTime)
                MessageBox.AddMessage("If this is your first time, press '?' for help.");
            if (m_engine.Player.SkillPoints > 0)
                MessageBox.AddMessage("You have skill points to spent. Press 's' to open the skill tree.");
            if (firstTime)
                MessageBox.AddMessage("Welcome To Magecrawl.");
            else
                MessageBox.AddMessage("Welcome Back To Magecrawl.");

            if (firstTime)
            {
                string text = "In the beginning the Creator created many worlds. Some, like this World, are malleable enough to allow sentient beings to force their will upon matter in limited ways. This is the foundation of magic.\n\nFor some unexplainable reason, you find yourself entering a small dungeon. Armed little beyond your wits, you've been drawn here to conquer.";
                OneButtonDialog d = new OneButtonDialog(this, text);
                d.Show();
            }
        }

        public void AddMessage(string s)
        {
            MessageBox.AddMessage(s);
        }

        private void OnKeyboardDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            m_currentKeystrokeHandler(KeyboardConverter.GetConvertedKey(e.Key, e.PlatformKeyCode), Map, this, m_engine);

            // We're the only one to handle keyboard messages
            e.Handled = true;
        }

        public void UpdateWorld()
        {
            Map.Draw();
            CharacterInfo.Character.NewTurn();
        }

        public void ResetDefaultKeyboardHandler()
        {
            m_currentKeystrokeHandler = m_defaultKey.OnKeyboardDown;
        }

        public void SetKeyboardHandler(KeystrokeHandler handler)
        {
            m_currentKeystrokeHandler = handler;
            UpdateWorld();
        }

        public void DisableFocusPopup()
        {
            m_focusPopupEnabled = false;
        }

        public void EnableFocusPopup()
        {
            m_focusPopupEnabled = true;
        }

        private void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (m_focusPopupEnabled)
                m_focusPopup.Show();
        }
    }
}
