using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.GameUI.Dialogs.Requests;

namespace Magecrawl.Keyboard.Dialogs
{
    internal class SaveGameKeyboardHandler : BaseKeystrokeHandler
    {
        public SaveGameKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest(new EnableSaveDialog(true));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new EnableSaveDialog(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void West()
        {
            m_gameInstance.SendPaintersRequest(new SaveDialogMoveLeft());
        }

        private void East()
        {
            m_gameInstance.SendPaintersRequest(new SaveDialogMoveRight());
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new SelectSave(new SaveSelected(SelectionDelegate)));
        }

        private void SelectionDelegate(bool shouldSave)
        {
            if (shouldSave)
            {
                m_engine.Save();
                m_gameInstance.IsQuitting = true;
            }
            Escape();
        }
    }
}
