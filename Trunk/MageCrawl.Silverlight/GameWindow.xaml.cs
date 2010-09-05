using System.Windows.Controls;
using Magecrawl.GameEngine.Interface;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight
{
    public partial class GameWindow : UserControl
    {
        private IGameEngine m_engine;

        public GameWindow()
        {
            InitializeComponent();
        }

        public void Setup(IGameEngine engine)
        {
            m_engine = engine;
            CharacterInfo.Setup(engine.Player);
            Map.Setup(engine.Map, engine.Player);
        }
    }
}
