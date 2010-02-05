using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Requests;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void OnOneButtonComplete();

    internal class OneButtonDialogKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private OnOneButtonComplete m_completeDelegate;

        public OneButtonDialogKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            OneButtonDialogKeyboardRequest dialogRequest = (OneButtonDialogKeyboardRequest)request;
            m_completeDelegate = dialogRequest.CompletionDelegate;
            m_gameInstance.SendPaintersRequest(new EnableOneButtonDialog(true, dialogRequest.Text));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new EnableOneButtonDialog(false, string.Empty));
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
