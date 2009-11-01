using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Dialogs
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

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
        }

        public override void DrawNewFrame(Console screen)
        {
            const int WelcomeScreenOffset = 13;
            if (m_enabled)
            {
                m_yesEnabled = TCODSystem.ElapsedSeconds > m_timeToEnableYes;

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

        public override void HandleRequest(string request, object data, object data2)
        {
            switch (request)
            {
                case "QuitDialogEnabled":
                    m_timeToEnableYes = TCODSystem.ElapsedSeconds + 2;
                    m_enabled = true;
                    break;
                case "QuitDialogDisabled":
                    m_enabled = false;
                    break;
                case "QuitDialogMoveLeft":
                    if (m_yesEnabled)
                        m_yesSelected = true;
                    break;
                case "QuitDialogMoveRight":
                    m_yesSelected = false;
                    break;
                case "QuitSelected":
                    QuitSelected del = (QuitSelected)data;
                    del(m_yesSelected);
                    break;
            }
        }
    }
}
