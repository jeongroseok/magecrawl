using System;

namespace Magecrawl.GameUI.SkillTree.Requests
{
    public class ShowSkillTree : RequestBase
    {
		private bool m_enabled;
		
        public ShowSkillTree(bool enable)
        {
			m_enabled = enable;
        }
		
		internal override void DoRequest (IHandlePainterRequest painter)
		{
			if(painter is SkillTreePainter)
			{
				((SkillTreePainter)painter).Enabled = m_enabled;
			}
		}
    }
}
