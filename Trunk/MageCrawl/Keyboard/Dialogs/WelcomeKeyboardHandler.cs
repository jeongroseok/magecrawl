using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using libtcod;

namespace Magecrawl.Keyboard.Dialogs
{
    internal class WelcomeKeyboardHandler : BaseKeystrokeHandler
    {
        public WelcomeKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest(new EnableWelcome(true));
            m_gameInstance.UpdatePainters();
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            if (keystroke.Code != TCODKeyCode.NoKey)
            {
                m_gameInstance.SendPaintersRequest(new EnableWelcome(false));
                m_gameInstance.UpdatePainters();
                m_gameInstance.ResetHandlerName();
            }
        }
    }
}
