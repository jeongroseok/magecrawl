using System.Collections.Generic;
using libtcodWrapper;

namespace Magecrawl.GameUI
{
    public sealed class DialogColorHelper
    {
        private Color m_savedForeground;
        private Color m_savedBackground;
        private bool m_stuffSaved;

        public DialogColorHelper()
        {
            m_stuffSaved = false;
        }

        public void ResetColors(Console screen)
        {
            if (m_stuffSaved)
            {
                screen.ForegroundColor = m_savedForeground;
                screen.BackgroundColor = m_savedBackground;
                m_stuffSaved = false;
            }
        }

        public void SaveColors(Console screen)
        {
            m_savedForeground = screen.ForegroundColor;
            m_savedBackground = screen.BackgroundColor;
            m_stuffSaved = true;
        }

        public void SetColors(Console screen, bool selected, bool enabled)
        {
            if (enabled)
            {
                if (selected)
                {
                    screen.ForegroundColor = ColorPresets.LightGray;
                    screen.BackgroundColor = TCODColorPresets.BrightBlue;
                }
                else
                {
                    screen.ForegroundColor = TCODColorPresets.Gray;
                    screen.BackgroundColor = TCODColorPresets.Black;
                }
            }
            else
            {
                if (selected)
                {
                    screen.ForegroundColor = TCODColorPresets.Red;
                    screen.BackgroundColor = TCODColorPresets.BrightYellow;
                }
                else
                {
                    screen.ForegroundColor = TCODColorPresets.Red;
                    screen.BackgroundColor = TCODColorPresets.Black;
                }
            }
        }
    }
}
