using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Controls;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.Interface;

namespace MageCrawl.Silverlight
{
    public partial class MainPage : UserControl
    {
        [Import]
        public IGameEngine m_engine;

        private CompositionContainer m_container;
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

        public void Compose()
        {
            // We need to add the ExecutingAssembly since it is an exe, it doesn't get added by the DirectoryCatalog
            m_container = new CompositionContainer(new AggregateCatalog(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly())));
            m_container.ComposeParts(this);
        }
    }
}
