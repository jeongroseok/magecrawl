using System.ComponentModel.Composition;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;

namespace Magecrawl.GameUI.Dialogs
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "QuitGame")]
    internal class QuitGameKeyboardHandler : BaseKeystrokeHandler
    {
        public override void NowPrimaried(object request)
        {
            QuitReason reason = (QuitReason)request;
            m_gameInstance.SendPaintersRequest(new EnableQuitDialog(reason));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new DisableQuitDialog());
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void West()
        {
            m_gameInstance.SendPaintersRequest(new QuitDialogMoveLeft());
        }

        private void East()
        {
            m_gameInstance.SendPaintersRequest(new QuitDialogMoveRight());
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new SelectQuit(new QuitSelected(SelectionDelegate)));
        }

        private void SelectionDelegate(bool shouldQuit)
        {
            if (shouldQuit)
            {
                m_gameInstance.IsQuitting = true;
            }
            Escape();
        }
    }
}
