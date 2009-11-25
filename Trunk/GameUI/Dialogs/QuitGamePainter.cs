using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void QuitSelected(bool shouldQuit);

    internal class QuitGamePainter : MapPainterBase
    {
        private bool m_enabled;
        private float m_timeToEnableYes;
        private DialogColorHelper m_dialogColorHelper;
        private bool m_yesSelected;
        private bool m_yesEnabled;

        public QuitGamePainter()
        {
            m_enabled = false;
            m_yesSelected = false;
            m_yesEnabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
        }

        public override void DrawNewFrame(Console screen)
        {
            const int WelcomeScreenOffset = 13;
            if (m_enabled)
            {
                m_yesEnabled = TCODSystem.ElapsedSeconds > m_timeToEnableYes;

                // Don't make debugger wait
                if (Preferences.Instance.DebuggingMode)
                    m_yesEnabled = true;

                m_dialogColorHelper.SaveColors(screen);
                screen.DrawFrame(WelcomeScreenOffset, WelcomeScreenOffset + 5, UIHelper.ScreenWidth - (2 * WelcomeScreenOffset), 11, true);
                string saveString = "Quitting the game will delete your current character. To stop playing now and continue your adventure later, use save instead.";
                screen.PrintLineRect(saveString, UIHelper.ScreenWidth / 2, 7 + WelcomeScreenOffset, UIHelper.ScreenWidth - 4 - (2 * WelcomeScreenOffset), UIHelper.ScreenHeight - (2 * WelcomeScreenOffset), LineAlignment.Center);

                screen.PrintLine("Really Quit?",  UIHelper.ScreenWidth / 2, 11 + WelcomeScreenOffset, LineAlignment.Center);

                m_dialogColorHelper.SetColors(screen, m_yesSelected, m_yesEnabled);
                screen.PrintLine("Yes", (UIHelper.ScreenWidth / 2) - 6, 13 + WelcomeScreenOffset, LineAlignment.Left);
                m_dialogColorHelper.SetColors(screen, !m_yesSelected, true);
                screen.PrintLine("No", (UIHelper.ScreenWidth / 2) + 4, 13 + WelcomeScreenOffset, LineAlignment.Left);

                m_dialogColorHelper.ResetColors(screen);
            }
        }

        internal void Enable()
        {
            m_timeToEnableYes = TCODSystem.ElapsedSeconds + 2;
            m_enabled = true;
        }

        internal void Disable()
        {
            m_enabled = false;
        }

        internal bool YesEnabled
        {
            get { return m_yesEnabled; }
        }

        internal bool YesSelected
        {
            get { return m_yesSelected; }
            set { m_yesSelected = value; }
        }

        internal void SelectQuit(QuitSelected onSelect)
        {
            onSelect(m_yesSelected);
        }
    }
}
