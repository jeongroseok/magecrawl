using System.Windows.Controls;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.Interface;

namespace MageCrawl.Silverlight
{
    public partial class MainPage : UserControl
    {
        public IGameEngine m_engine;

        private TextBlock m_status;
        public MainPage()
        {
            InitializeComponent();

            m_status = (TextBlock)FindName("Status");
            m_status.Text += "\nInit (C#)";

            m_engine = new PublicGameEngineInterface();

            //Compose();

            //m_status.Text = "Compose Complete!";

            m_status.Text += "\nHack Complete!";

            m_engine.PlayerDiedEvent += new PlayerDied(PlayerDiedEvent);
            m_engine.RangedAttackEvent += new RangedAttack(RangedAttackEvent);
            m_engine.TextOutputEvent += new TextOutputFromGame(TextOutputEvent);

            m_status.Text += "\nCreating World!";

            m_engine.CreateNewWorld("Donblas", "Scholar");

            m_status.Text += "\nWorld Created!";
        }

        void TextOutputEvent(string s)
        {
            m_status.Text += "\nGame Output - " + s;
        }

        void RangedAttackEvent(object attackingMethod, ShowRangedAttackType type, object data, bool targetAtEndPoint)
        {
            m_status.Text += "\nRanged Attack?";
        }

        void PlayerDiedEvent()
        {
            m_status.Text += "\nPlayer Died?";
        }
    }
}
