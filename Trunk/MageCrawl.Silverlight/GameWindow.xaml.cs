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

        private KeystrokeHandler m_currentKeystrokeHandler;

        public GameWindow()
        {
            InitializeComponent();
            m_currentKeystrokeHandler = DefaultKeyboardHandler.OnKeyboardDown;
        }

        public void Setup(IGameEngine engine)
        {
            m_engine = engine;
            CharacterInfo.Setup(engine.Player);
            Map.Setup(engine);
            MessageBox.AddMessage("Welcome to Magecrawl");

            // Gives SL focus on startup!
            System.Windows.Browser.HtmlPage.Plugin.Focus();

            m_focusPopup = new LostFocusPopup();
            m_focusPopupEnabled = true;
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
            m_currentKeystrokeHandler = DefaultKeyboardHandler.OnKeyboardDown;
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
