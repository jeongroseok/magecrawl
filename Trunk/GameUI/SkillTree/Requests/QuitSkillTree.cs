using System;
using System.Collections.Generic;
using Magecrawl.GameUI;
using Magecrawl.GameUI.SkillTree;
using Magecrawl.Interfaces;

namespace Magecrawl.GameUI.SkillTree.Requests
{
    public class QuitSkillTree : RequestBase
    {
        public delegate void QuitDelegate(List<ISkill> newlySelectedSkills);

        private QuitDelegate m_delegate;

        public QuitSkillTree(QuitDelegate quitDelegate)
        {
            m_delegate = quitDelegate;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            SkillTreePainter skillTreePainter = painter as SkillTreePainter;
            if(skillTreePainter != null)
            {
                m_delegate(skillTreePainter.NewlySelectedSkills);
                skillTreePainter.Enabled = false;
            }
        }
    }
}
