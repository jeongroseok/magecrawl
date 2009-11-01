using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.Map.Requests;

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
            if (l != null)
            {
                m_onSelected(l.CurrentSelection);
            }
        }
    }
}
