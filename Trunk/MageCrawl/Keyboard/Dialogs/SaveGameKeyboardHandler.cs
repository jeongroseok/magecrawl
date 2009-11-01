using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Map.Dialogs;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.Dialogs
{
    internal class SaveGameKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public SaveGameKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            m_gameInstance.SendPaintersRequest("SaveDialogEnabled");
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest("SaveDialogDisabled");
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void West()
        {
            m_gameInstance.SendPaintersRequest("SaveDialogMoveLeft");
        }

        private void East()
        {
            m_gameInstance.SendPaintersRequest("SaveDialogMoveRight");
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest("SaveSelected", new SaveSelected(SelectionDelegate));
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
