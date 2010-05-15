using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameUI.Inventory.Requests
{
    public class ShowInventoryItemWindow : RequestBase
    {
        private bool m_show;
        private IItem m_data;
        private List<ItemOptions> m_options;

        public ShowInventoryItemWindow(bool show)
        {
            m_show = show;
            m_data = null;
            m_options = null;

            if (m_show)
                throw new System.ArgumentException("ShowInventoryItemWindow(bool show) must only be called if show is false");
        }

        public ShowInventoryItemWindow(bool show, IItem data, List<ItemOptions> options)
        {
            m_show = show;
            m_data = data;
            m_options = options;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            InventoryItemPainter p = painter as InventoryItemPainter;
            if (p != null)
            {
                if (m_show)
                    p.Show(m_data, m_options);
                else
                    p.Hide();
            }
        }
    }
}
