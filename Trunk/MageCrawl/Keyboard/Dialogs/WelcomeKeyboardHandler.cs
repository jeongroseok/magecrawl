using System.ComponentModel.Composition;
using libtcod;
using Magecrawl.GameUI.Dialogs.Requests;

namespace Magecrawl.Keyboard.Dialogs
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "Welcome")]
    internal class WelcomeKeyboardHandler : BaseKeystrokeHandler
    {
        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest(new EnableWelcome(true));
            m_gameInstance.UpdatePainters();
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            if (keystroke.Code != TCODKeyCode.NoKey)
            {
                m_gameInstance.SendPaintersRequest(new EnableWelcome(false));
                m_gameInstance.UpdatePainters();
                m_gameInstance.ResetHandlerName();
            }
        }
    }
}
