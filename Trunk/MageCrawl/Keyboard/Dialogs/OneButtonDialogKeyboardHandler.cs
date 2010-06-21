using Magecrawl.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void OnOneButtonComplete();

    internal class OneButtonDialogKeyboardHandler : BaseKeystrokeHandler
    {
        private OnOneButtonComplete m_completeDelegate;

        public OneButtonDialogKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

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
