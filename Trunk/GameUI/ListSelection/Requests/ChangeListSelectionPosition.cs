using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class ChangeListSelectionPosition : RequestBase
    {
        private Direction m_direction;

        public ChangeListSelectionPosition(Direction direction)
        {
            m_direction = direction;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter l = painter as ListSelectionPainter;
            if (l != null)
            {
                l.MoveInventorySelection(m_direction);
            }
        }
    }
}
