using System.Windows;
using System.Windows.Controls;
using Magecrawl;
using Magecrawl.GameEngine.Interface;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight
{
    public partial class GameWindow : UserControl
    {
        private IGameEngine m_engine;
        private LostFocusPopup m_focusPopup;

        public GameWindow()
        {
            InitializeComponent();
        }

        public void Setup(IGameEngine engine)
        {
            m_engine = engine;
            CharacterInfo.Setup(engine.Player);
            Map.Setup(engine.Map, engine.Player);
            MessageBox.AddMessage("Welcome to Magecrawl");
            
            // Silverlight by default doesn't give us focus :(
            m_focusPopup = new LostFocusPopup();
            m_focusPopup.Show();
        }

        public void AddMessage(string s)
        {
            MessageBox.AddMessage(s);
        }

        private void OnKeyboardDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Left:
                case System.Windows.Input.Key.A:
                    HandleDirection(Direction.West);
                    break;
                case System.Windows.Input.Key.Right:
                case System.Windows.Input.Key.D:
                    HandleDirection(Direction.East);
                    break;
                case System.Windows.Input.Key.Down:
                case System.Windows.Input.Key.S:
                    HandleDirection(Direction.South);
                    break;
                case System.Windows.Input.Key.Up:
                case System.Windows.Input.Key.W:
                    HandleDirection(Direction.North);
                    break;
                default:
                    break;
            }
            // We're the only one to handle keyboard messages
            e.Handled = true;
        }

        private void HandleDirection(Magecrawl.Direction direction)
        {
            m_engine.Actions.Move(direction);
            UpdateWorld();
        }

        private void UpdateWorld()
        {
            Map.Draw();
            CharacterInfo.Character.NewTurn();
        }

        private void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            m_focusPopup.Show();
        }
    }
}
