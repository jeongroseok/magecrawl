using libtcod;
using Magecrawl.GameUI.Utilities;

namespace Magecrawl.GameUI
{
    public sealed class DialogColorHelper
    {
        private TCODColor m_savedForeground;
        private TCODColor m_savedBackground;
        private bool m_stuffSaved;

        public DialogColorHelper()
        {
            m_stuffSaved = false;
        }

        public void ResetColors(TCODConsole screen)
        {
            if (m_stuffSaved)
            {
                screen.setForegroundColor(m_savedForeground);
                screen.setBackgroundColor(m_savedBackground);
                m_stuffSaved = false;
            }
        }

        public void SaveColors(TCODConsole screen)
        {
            m_savedForeground = screen.getForegroundColor();
            m_savedBackground = screen.getBackgroundColor();
            m_stuffSaved = true;
        }

        public void SetColors(TCODConsole screen, bool selected, bool enabled)
        {
            if (enabled)
            {
                if (selected)
                {
                    screen.setForegroundColor(ColorPresets.LightGray);
                    screen.setBackgroundColor(ColorPresetsFromTCOD.BrightBlue);
                }
                else
                {
                    screen.setForegroundColor(ColorPresets.Gray);
                    screen.setBackgroundColor(ColorPresets.Black);
                }
            }
            else
            {
                if (selected)
                {
                    screen.setForegroundColor(ColorPresets.Red);
                    screen.setBackgroundColor(ColorPresetsFromTCOD.BrightYellow);
                }
                else
                {
                    screen.setForegroundColor(ColorPresets.Red);
                    screen.setBackgroundColor(ColorPresets.Black);
                }
            }
        }
    }
}
