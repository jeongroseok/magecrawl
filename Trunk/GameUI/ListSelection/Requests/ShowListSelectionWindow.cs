using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class ShowListSelectionWindow : RequestBase
    {
        private bool m_show;
        private List<INamedItem> m_data;
        private string m_title;

        public ShowListSelectionWindow(bool enable)
        {
            m_show = enable;
            m_data = null;
            m_title = null;

            if (m_show)
                throw new System.ArgumentException("ShowListSelectionWindow(bool show) must only be called if show is false");        
        }

        public ShowListSelectionWindow(bool enable, List<INamedItem> data, string title)
        {
            m_show = enable;
            m_data = data;
            m_title = title;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter l = painter as ListSelectionPainter;
            if (l != null)
            {
                if (m_show)
                    l.Enable(m_data, m_title);
                else
                    l.Disable();
            }
        }
    }
}
