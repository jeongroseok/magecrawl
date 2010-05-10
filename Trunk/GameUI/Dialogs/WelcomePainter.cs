using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class WelcomePainter : MapPainterBase
    {
        internal bool Enabled { get; set; }

        public WelcomePainter()
        {
            Enabled = false;
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            const int WelcomeScreenOffset = 13;
            if (Enabled)
            {
                screen.printFrame(WelcomeScreenOffset, WelcomeScreenOffset + 5, UIHelper.ScreenWidth - (2 * WelcomeScreenOffset), 14, true);
                string welcomeString = "In the beginning the Creator created many worlds. Some, like this World, are malleable enough to allow sentient beings to force their will upon matter in limited ways. This is the foundation of magic.\n\nFor some unexplainable reason, you find yourself entering a small dungeon. Armed little beyond your wits, you've been drawn here to conquer.\n\n--Press Any Key to Begin--";
                screen.printRectEx(UIHelper.ScreenWidth / 2, 6 + WelcomeScreenOffset, UIHelper.ScreenWidth - 4 - (2 * WelcomeScreenOffset), UIHelper.ScreenHeight - (2 * WelcomeScreenOffset), TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, welcomeString);
            }
        }
    }
}
