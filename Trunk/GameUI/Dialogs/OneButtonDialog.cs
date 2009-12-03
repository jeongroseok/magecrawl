using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class OneButtonDialog : MapPainterBase
    {
        private bool m_enabled;
        private string m_text;
        private DialogColorHelper m_dialogColorHelper;

        public OneButtonDialog()
        {
            m_enabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public void Enable(string textToDisplay)
        {
            m_text = textToDisplay;
            m_enabled = true;
        }

        public void Disable()
        {
            m_enabled = false;
        }

        public override void DrawNewFrame(Console screen)
        {
            const int DialogOffset = 13;
            if (m_enabled)
            {
                m_dialogColorHelper.SaveColors(screen);
                screen.DrawFrame(DialogOffset, DialogOffset + 5, UIHelper.ScreenWidth - (2 * DialogOffset), 11, true);

                screen.PrintLineRect(m_text, UIHelper.ScreenWidth / 2, 7 + DialogOffset, UIHelper.ScreenWidth - 4 - (2 * DialogOffset), UIHelper.ScreenHeight - (2 * DialogOffset), LineAlignment.Center);

                m_dialogColorHelper.SetColors(screen, true, true);
                screen.PrintLine("OK", UIHelper.ScreenWidth / 2, 13 + DialogOffset, LineAlignment.Center);

                m_dialogColorHelper.ResetColors(screen);
            }
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {   
        }

        public override void Dispose()
        {
        }
    }
}
