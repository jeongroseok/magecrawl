using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    public delegate void OnTwoButtonComplete(bool ok);

    internal class TwoButtonDialogKeyboardHandler : BaseKeystrokeHandler
    {
        private OnTwoButtonComplete m_completeDelegate;

        public TwoButtonDialogKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            var requestData = (Pair<OnTwoButtonComplete, List<string>>)request;
            m_completeDelegate = requestData.First;
            m_gameInstance.SendPaintersRequest(new EnableTwoButtonDialog(true, requestData.Second[0]));
            m_gameInstance.SendPaintersRequest(new ChangeTwoButtonText(requestData.Second[1], requestData.Second[2]));
            m_gameInstance.UpdatePainters();
        }

        private void West()
        {
            m_gameInstance.SendPaintersRequest(new MoveTwoButtonDialogSelection(true));
        }

        private void East()
        {
            m_gameInstance.SendPaintersRequest(new MoveTwoButtonDialogSelection(false));
        }
        
        private void Select()
        {
            bool leftButtonSelected = false;
            // Sucks out the left button status and places it in leftButtonSelected
            m_gameInstance.SendPaintersRequest(new DisableTwoButtonDialog((left) => leftButtonSelected = left));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
            m_completeDelegate(leftButtonSelected);
        }
    }
}
