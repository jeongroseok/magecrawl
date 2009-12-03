using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

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

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            string text = (string)objOne;
            m_completeDelegate = (OnOneButtonComplete)objTwo;
            m_gameInstance.SendPaintersRequest(new EnableOneButtonDialog(true, text));
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
