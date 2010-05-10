using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class OneButtonDialog : MapPainterBase
    {
        internal bool Enabled { get; set;}
        internal string Text { get; set;}

        private DialogColorHelper m_dialogColorHelper;

        public OneButtonDialog()
        {
            Enabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            const int DialogOffset = 13;
            if (Enabled)
            {
                m_dialogColorHelper.SaveColors(screen);
                screen.printFrame(DialogOffset, DialogOffset + 5, UIHelper.ScreenWidth - (2 * DialogOffset), 11, true);

                screen.printRectEx(UIHelper.ScreenWidth / 2, 7 + DialogOffset, UIHelper.ScreenWidth - 4 - (2 * DialogOffset), UIHelper.ScreenHeight - (2 * DialogOffset), TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, Text);

                m_dialogColorHelper.SetColors(screen, true, true);
                screen.printEx(UIHelper.ScreenWidth / 2, 13 + DialogOffset, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, "OK");

                m_dialogColorHelper.ResetColors(screen);
            }
        }
    }
}
