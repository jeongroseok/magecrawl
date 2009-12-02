using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class QuitGameKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public QuitGameKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            QuitReason reason = (QuitReason)objOne;
            m_gameInstance.SendPaintersRequest(new EnableQuitDialog(reason));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new DisableDialog());
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
