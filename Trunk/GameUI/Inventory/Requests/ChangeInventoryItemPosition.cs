using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameUI.Inventory.Requests
{
    public class ChangeInventoryItemPosition : RequestBase
    {
        private Direction m_direction;

        public ChangeInventoryItemPosition(Direction direction)
        {
            m_direction = direction;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            InventoryItemPainter p = painter as InventoryItemPainter;
            if (p != null)
            {
                p.ChangeSelectionPosition(m_direction);
            }
        }
    }
}
