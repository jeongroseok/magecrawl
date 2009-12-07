using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void SaveSelected(bool shouldSave);

    internal class SaveGamePainter : MapPainterBase
    {
        private bool m_enabled;
        private float m_timeToEnableYes;
        private DialogColorHelper m_dialogColorHelper;
        private bool m_yesSelected;
        private bool m_yesEnabled;

        public SaveGamePainter()
        {
            m_enabled = false;
            m_yesSelected = false;
            m_yesEnabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public override void DrawNewFrame(Console screen)
        {
            const int WelcomeScreenOffset = 13;
            if (m_enabled)
            {
                m_yesEnabled = TCODSystem.ElapsedSeconds > m_timeToEnableYes;

                if (Preferences.Instance.DebuggingMode)
                    m_yesEnabled = true;

                m_dialogColorHelper.SaveColors(screen);
                screen.DrawFrame(WelcomeScreenOffset, WelcomeScreenOffset + 5, UIHelper.ScreenWidth - (2 * WelcomeScreenOffset), 11, true);
                string saveString = "Saving the game will end your current session and allow you to pickup playing later.";
                screen.PrintLineRect(saveString, UIHelper.ScreenWidth / 2, 7 + WelcomeScreenOffset, UIHelper.ScreenWidth - 4 - (2 * WelcomeScreenOffset), UIHelper.ScreenHeight - (2 * WelcomeScreenOffset), LineAlignment.Center);

                screen.PrintLine("Really Save?",  UIHelper.ScreenWidth / 2, 11 + WelcomeScreenOffset, LineAlignment.Center);

                m_dialogColorHelper.SetColors(screen, m_yesSelected, m_yesEnabled);
                screen.PrintLine("Yes", (UIHelper.ScreenWidth / 2) - 6, 13 + WelcomeScreenOffset, LineAlignment.Left);
                m_dialogColorHelper.SetColors(screen, !m_yesSelected, true);
                screen.PrintLine("No", (UIHelper.ScreenWidth / 2) + 4, 13 + WelcomeScreenOffset, LineAlignment.Left);

                m_dialogColorHelper.ResetColors(screen);
            }
        }

        internal void Enable()
        {
            m_timeToEnableYes = TCODSystem.ElapsedSeconds + 1;
            m_enabled = true;
        }

        internal void Disable()
        {
            m_enabled = false;
        }

        internal bool SaveEnabled
        {
            get { return m_yesEnabled; }
        }

        internal bool SaveSelected
        {
            get { return m_yesSelected; }
            set { m_yesSelected = value; }
        }

        internal void SelectSave(SaveSelected onSelect)
        {
            onSelect(m_yesSelected);
        }
    }
}
