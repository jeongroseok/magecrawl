using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class TwoButtonDialog : MapPainterBase
    {
        internal string Text { get ; set; }

        private DialogColorHelper m_dialogColorHelper;
        private float m_timeToEnableYes;
        private bool m_leftSelected;
        private bool m_leftEnabled;
        private string m_leftButton;
        private string m_rightButton;

        public TwoButtonDialog()
        {
            Enabled = false;
            m_timeToEnableYes = 0;
            m_leftEnabled = false;
            m_leftSelected = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        private bool m_enabled;
        internal bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
                m_timeToEnableYes = TCODSystem.getElapsedSeconds() + 2;
                m_leftEnabled = false;
                m_leftSelected = false;
                SetupDefaultText();
            }
        }

        public bool LeftButtonSelected
        {
            get
            {
                return m_leftSelected;
            }
        }

        public void MoveSelection(bool left)
        {
            if(left && m_leftEnabled)
                m_leftSelected = true;
            else if (!left)
                m_leftSelected = false;
        }

        private void SetupDefaultText()
        {
            ChangeButtonText("Yes", "No");
        }

        public void ChangeButtonText(string left, string right)
        {
            m_leftButton = left;
            m_rightButton = right;
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            const int DialogOffset = 13;
            if (Enabled)
            {
                m_leftEnabled = TCODSystem.getElapsedSeconds() > m_timeToEnableYes;

                if (Preferences.Instance.DebuggingMode)
                    m_leftEnabled = true;

                m_dialogColorHelper.SaveColors(screen);
                screen.printFrame(DialogOffset, DialogOffset + 5, UIHelper.ScreenWidth - (2 * DialogOffset), 9, true);

                screen.printRectEx(UIHelper.ScreenWidth / 2, 7 + DialogOffset, UIHelper.ScreenWidth - 4 - (2 * DialogOffset), UIHelper.ScreenHeight - (2 * DialogOffset) - 2, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, Text);

                m_dialogColorHelper.SetColors(screen, m_leftSelected, m_leftEnabled);
                screen.print((UIHelper.ScreenWidth / 2) - 13, 11 + DialogOffset, m_leftButton);
                m_dialogColorHelper.SetColors(screen, !m_leftSelected, true);
                screen.print((UIHelper.ScreenWidth / 2) + 5, 11 + DialogOffset, m_rightButton);

                m_dialogColorHelper.ResetColors(screen);
            }
        }
    }
}
