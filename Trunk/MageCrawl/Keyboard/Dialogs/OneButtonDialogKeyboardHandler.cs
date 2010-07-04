using System.ComponentModel.Composition;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void OnOneButtonComplete();

    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "OneButtonDialog")]
    internal class OneButtonDialogKeyboardHandler : BaseKeystrokeHandler
    {
        private OnOneButtonComplete m_completeDelegate;

        public override void NowPrimaried(object request)
        {
            var requestData = (Pair<OnOneButtonComplete, string>)request;
            m_completeDelegate = requestData.First;
            m_gameInstance.SendPaintersRequest(new EnableOneButtonDialog(true, requestData.Second));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new EnableOneButtonDialog(false, ""));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
            m_completeDelegate();
        }

        private void Select()
        {
            Escape();
        }
    }
}
