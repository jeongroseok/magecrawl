using System;

namespace Magecrawl.GameUI.SkillTree.Requests
{
    public class ShowSkillTree : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            if (painter is SkillTreePainter)
            {
                ((SkillTreePainter)painter).Enabled = true;
            }
        }
    }
}
