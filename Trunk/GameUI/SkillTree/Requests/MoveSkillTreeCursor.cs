using System;
using Magecrawl.GameUI.SkillTree;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree.Requests
{
    public class MoveSkillTreeCursor : RequestBase
    {
        private Direction m_direction;

        public MoveSkillTreeCursor(Direction direction)
        {
            m_direction = direction;
        }

        internal override void DoRequest (IHandlePainterRequest painter)
        {
            if(painter is SkillTreePainter)
            {
                Point cursorPosition = ((SkillTreePainter)painter).CursorPosition;
                ((SkillTreePainter)painter).CursorPosition = PointDirectionUtils.ConvertDirectionToDestinationPoint(cursorPosition, m_direction);
            }
        }
    }
}
