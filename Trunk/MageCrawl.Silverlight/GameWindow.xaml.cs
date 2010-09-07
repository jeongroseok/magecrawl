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
            Map.Setup(engine, engine.Map, engine.Player);
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
                    HandleDirection(Direction.West);
                    break;
                case System.Windows.Input.Key.Right:
                    HandleDirection(Direction.East);
                    break;
                case System.Windows.Input.Key.Down:
                    HandleDirection(Direction.South);
                    break;
                case System.Windows.Input.Key.Up:
                    HandleDirection(Direction.North);
                    break;
                case System.Windows.Input.Key.Insert:
                    HandleDirection(Direction.Northwest);
                    break;
                case System.Windows.Input.Key.Delete:
                    HandleDirection(Direction.Southwest);
                    break;
                case System.Windows.Input.Key.PageUp:
                    HandleDirection(Direction.Northeast);
                    break;
                case System.Windows.Input.Key.PageDown:
                    HandleDirection(Direction.Southeast);
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
