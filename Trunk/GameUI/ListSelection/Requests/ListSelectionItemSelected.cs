using System;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class ListSelectionItemSelected : RequestBase
    {
        private ListItemSelected m_onSelected;

        public ListSelectionItemSelected(ListItemSelected onSelected)
        {
            m_onSelected = onSelected;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter l = painter as ListSelectionPainter;
            if (l != null && l.IsEnabled(l.CurrentSelection))
                m_onSelected(l.CurrentSelection);
        }
    }
}
