using System;

namespace Magecrawl.GameUI.Inventory.Requests
{
    public class SelectInventoryItemOption : RequestBase
    {
        private InventoryItemOptionSelected m_onSelect;

        public SelectInventoryItemOption(InventoryItemOptionSelected onSelect)
        {
            m_onSelect = onSelect;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            InventoryItemPainter p = painter as InventoryItemPainter;
            if (p != null)
                p.SelectOptionOnCurrent(m_onSelect);
        }
    }
}
