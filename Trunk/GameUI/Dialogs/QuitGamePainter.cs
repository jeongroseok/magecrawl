using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void QuitSelected(bool shouldQuit);

    public enum QuitReason
    {
        quitAction, leaveDungeom
    }

    internal class QuitGamePainter : MapPainterBase
    {
        private bool m_enabled;
        private float m_timeToEnableYes;
        private DialogColorHelper m_dialogColorHelper;
        private bool m_yesSelected;
        private bool m_yesEnabled;
        private QuitReason m_quitReason;

        public QuitGamePainter()
        {
            m_enabled = false;
            m_yesSelected = false;
            m_yesEnabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            const int WelcomeScreenOffset = 13;
            if (m_enabled)
            {
                m_yesEnabled = TCODSystem.getElapsedSeconds() > m_timeToEnableYes;

                // Don't make debugger wait
                if (Preferences.Instance.DebuggingMode)
                    m_yesEnabled = true;

                m_dialogColorHelper.SaveColors(screen);
                screen.printFrame(WelcomeScreenOffset, WelcomeScreenOffset + 5, UIHelper.ScreenWidth - (2 * WelcomeScreenOffset), 11, true);
                string quitString;
                if (m_quitReason == QuitReason.quitAction)
                    quitString = "Quitting the game will delete your current character. To stop playing now and continue your adventure later, use save instead.";
                else
                    quitString = "Leaving the dungeon will end the game early and delete your current character. To stop playing now and continue your adventure later, use save instead.";

                screen.printRectEx(UIHelper.ScreenWidth / 2, 7 + WelcomeScreenOffset, UIHelper.ScreenWidth - 4 - (2 * WelcomeScreenOffset), UIHelper.ScreenHeight - (2 * WelcomeScreenOffset), TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, quitString);

                screen.printEx(UIHelper.ScreenWidth / 2, 11 + WelcomeScreenOffset, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, "Really Quit?");

                m_dialogColorHelper.SetColors(screen, m_yesSelected, m_yesEnabled);
                screen.print((UIHelper.ScreenWidth / 2) - 6, 13 + WelcomeScreenOffset, "Yes");
                m_dialogColorHelper.SetColors(screen, !m_yesSelected, true);
                screen.print((UIHelper.ScreenWidth / 2) + 4, 13 + WelcomeScreenOffset, "No");

                m_dialogColorHelper.ResetColors(screen);
            }
        }

        internal void Enable(QuitReason reason)
        {
            m_timeToEnableYes = TCODSystem.getElapsedSeconds() + 1;
            m_enabled = true;
            m_quitReason = reason;
        }

        internal void Disable()
        {
            m_yesSelected = false;
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
