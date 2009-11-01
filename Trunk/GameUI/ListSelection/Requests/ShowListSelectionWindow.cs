using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class ShowListSelectionWindow : RequestBase
    {
        private bool m_enable;
        private List<INamedItem> m_data;
        private string m_title;

        public ShowListSelectionWindow(bool enable, List<INamedItem> data, string title)
        {
            m_enable = enable;
            m_data = data;
            m_title = title;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter l = painter as ListSelectionPainter;
            if (l != null)
            {
                if (m_enable)
                {
                    l.Enable(m_data, m_title);
                }
                else
                {
                    l.Disable();
                }
            }
        }
    }
}
