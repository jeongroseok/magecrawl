using System.Windows.Controls;
using Magecrawl.GameEngine.Interface;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight
{
    public partial class GameWindow : UserControl
    {
        private IGameEngine m_engine;
        private MessageBox m_messages;
        private CharacterInfo m_characterInfo;
        private Map m_map;

        public GameWindow()
        {
            InitializeComponent();
            m_messages = (MessageBox)FindName("MessageBox");
            m_characterInfo = (CharacterInfo)FindName("CharacterInfo");
            m_map = (Map)FindName("Map");
        }

        public void Setup(IGameEngine engine)
        {
            m_engine = engine;
            m_characterInfo.Setup(engine.Player);
            m_map.Setup(engine.Map);
        }
    }
}
