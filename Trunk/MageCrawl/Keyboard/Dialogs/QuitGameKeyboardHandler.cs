using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.GameUI.Map.Dialogs;

namespace Magecrawl.Keyboard.Dialogs
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
            m_gameInstance.SendPaintersRequest("QuitDialogEnabled");
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest("QuitDialogDisabled");
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void West()
        {
            m_gameInstance.SendPaintersRequest("QuitDialogMoveLeft");
        }

        private void East()
        {
            m_gameInstance.SendPaintersRequest("QuitDialogMoveRight");
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest("QuitSelected", new QuitSelected(SelectionDelegate));
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
