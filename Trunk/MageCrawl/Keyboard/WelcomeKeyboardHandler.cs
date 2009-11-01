using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class WelcomeKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public WelcomeKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            m_gameInstance.SendPaintersRequest("WelcomeEnabled");
            m_gameInstance.UpdatePainters();
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            if (keystroke.Code != libtcodWrapper.KeyCode.TCODK_NONE)
            {
                m_gameInstance.SendPaintersRequest("WelcomeDisabled");
                m_gameInstance.UpdatePainters();
                m_gameInstance.ResetHandlerName();
            }
        }
    }
}
