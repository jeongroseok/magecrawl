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

        public GameWindow(IGameEngine gameEngine)
        {
            InitializeComponent();

            m_engine = gameEngine;
            m_messages = (MessageBox)FindName("MessageBox");
            m_characterInfo = (CharacterInfo)FindName("CharacterInfo");
            m_map = (Map)FindName("Map");
        }
    }
}
