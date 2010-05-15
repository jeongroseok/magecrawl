using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.GameUI.SkillTree.Requests;
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

        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest(new ShowSkillTree());
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new SelectSkillFromSkillTree());
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new QuitSkillTree(QuitDelegate));
        }

        protected override void ChangeTabs()
        {
            m_gameInstance.SendPaintersRequest(new ChangeSkillTab(false));
        }

        protected override void ShiftChangeTabs()
        {
            m_gameInstance.SendPaintersRequest(new ChangeSkillTab(true));
        }

        private class DialogConfirmAction
        {
            private GameInstance m_gameInstance;
            private IGameEngine m_engine;
            private List<ISkill> m_newlySelectedSkills;

            internal DialogConfirmAction(IGameEngine engine, GameInstance gameInstance, List<ISkill> newlySelectedSkill)
            {
                m_engine = engine;
                m_gameInstance = gameInstance;
                m_newlySelectedSkills = new List<ISkill>(newlySelectedSkill);   // Copy list since skill tree will clear
            }

            internal void OnConfirm(bool ok)
            {
                if (ok)
                {
                    foreach (ISkill s in m_newlySelectedSkills)
                        m_engine.AddSkillToPlayer(s);
                }

                m_gameInstance.UpdatePainters();
                m_gameInstance.ResetHandlerName();
            }
        }

        private void QuitDelegate(List<ISkill> newlySelectedSkill)
        {
            if (newlySelectedSkill.Count > 0)
            {
                DialogConfirmAction action = new DialogConfirmAction(m_engine, m_gameInstance, newlySelectedSkill);
                
                // TODO - Handle skill point cost here
                string dialogString = string.Format("You have selected {0} new skill{1}.\n\nTotal cost: {2} skill point{1}.",
                                                    newlySelectedSkill.Count, newlySelectedSkill.Count > 1 ? "s" : "",
                                                    newlySelectedSkill.Select(x => x.Cost).Sum());
                List<string> dialogStringList = new List<string>() { dialogString, "Select Skills", "Cancel" };
                var dialogInfo = new Pair<OnTwoButtonComplete, List<string>>(action.OnConfirm, dialogStringList);
                m_gameInstance.SetHandlerName("TwoButtonDialog", dialogInfo);
            }
            else
            {
                m_gameInstance.UpdatePainters();
                m_gameInstance.ResetHandlerName();
            }
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
