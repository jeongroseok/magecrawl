using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class WelcomePainter : MapPainterBase
    {
        private bool m_enabled;

        public WelcomePainter()
        {
            m_enabled = false;
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
                screen.DrawFrame(WelcomeScreenOffset, WelcomeScreenOffset + 5, UIHelper.ScreenWidth - (2 * WelcomeScreenOffset), 14, true);
                string welcomeString = "In the beginning the Creator created many worlds. Some, like this World, are malleable enough to allow sentient beings to force their will upon matter in limited ways. This is the foundation of magic.\n\nFor some unexplainable reason, you find yourself entering a small dungeon. Armed little beyond your wits, you've been drawn here to conquer.\n\n--Press Any Key to Begin--";
                screen.PrintLineRect(welcomeString, UIHelper.ScreenWidth / 2, 6 + WelcomeScreenOffset, UIHelper.ScreenWidth - 4 - (2 * WelcomeScreenOffset), UIHelper.ScreenHeight - (2 * WelcomeScreenOffset), LineAlignment.Center);
            }
        }

        internal bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }
    }
}
