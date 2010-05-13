using Magecrawl.GameUI.SkillTree;

namespace Magecrawl.GameUI.SkillTree.Requests
{
    public class ChangeSkillTab : RequestBase
    {
        private bool m_moveLeft;

        public ChangeSkillTab(bool moveLeft)
        {
            m_moveLeft = moveLeft;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            SkillTreePainter skillTreePainter = painter as SkillTreePainter;
            if (skillTreePainter != null)
            {
                if (m_moveLeft)
                    skillTreePainter.DecrementCurrentTab();
                else
                    skillTreePainter.IncrementCurrentTab();
            }
        }
    }
}
