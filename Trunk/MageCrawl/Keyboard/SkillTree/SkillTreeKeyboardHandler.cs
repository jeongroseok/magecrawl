using System;
using Magecrawl.Keyboard;
using Magecrawl.GameUI.SkillTree.Requests;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.SkillTree
{
    internal class SkillTreeKeyboardHandler : BaseKeystrokeHandler
    {
        public SkillTreeKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried (object request)
        {
            m_gameInstance.SendPaintersRequest(new ShowSkillTree(true));
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowSkillTree(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }
    }
}
