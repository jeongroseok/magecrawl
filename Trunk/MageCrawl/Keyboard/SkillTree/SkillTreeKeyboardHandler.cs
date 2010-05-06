using System;
using System.Collections.Generic;
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

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new SelectSkillFromSkillTree());
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowSkillTree(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void North()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.North));
        }

        private void South()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.South));
        }

        private void East()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.East));
        }

        private void West()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.West));
        }

        private void Northeast()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.Northeast));
        }

        private void Northwest()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.Northwest));
        }

        private void Southeast()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.Southeast));
        }

        private void Southwest()
        {
            m_gameInstance.SendPaintersRequest(new MoveSkillTreeCursor(Direction.Southwest));
        }

    }
}
