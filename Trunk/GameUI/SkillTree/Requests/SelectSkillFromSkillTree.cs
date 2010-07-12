using System;
using Magecrawl.GameUI;

namespace Magecrawl.GameUI.SkillTree.Requests
{
    public class SelectSkillFromSkillTree : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            if (painter is SkillTreePainter)
            {
                ((SkillTreePainter)painter).SelectSquare();
            }
        }
    }
}
