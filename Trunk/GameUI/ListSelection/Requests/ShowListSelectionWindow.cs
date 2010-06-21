using System.Collections.Generic;
using Magecrawl.Interfaces;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class ShowListSelectionWindow : RequestBase
    {
        private bool m_show;
        private List<INamedItem> m_data;
        private string m_title;
        private ListItemShouldBeEnabled m_selectionDelegate;
        private bool m_useLetters;

        public ShowListSelectionWindow(bool enable)
        {
            m_show = enable;
            m_data = null;
            m_title = null;

            if (m_show)
                throw new System.ArgumentException("ShowListSelectionWindow(bool show) must only be called if show is false");        
        }

        public ShowListSelectionWindow(bool enable, List<INamedItem> data,  bool useLetters, string title)
            : this(enable, data, useLetters, title, i => { return true; })
        {
        }

        public ShowListSelectionWindow(bool enable, List<INamedItem> data, bool useLetters, string title, ListItemShouldBeEnabled selectionDelegate)
        {
            m_show = enable;
            m_data = data;
            m_title = title;
            m_useLetters = useLetters;
            m_selectionDelegate = selectionDelegate;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter l = painter as ListSelectionPainter;
            if (l != null)
            {
                if (m_show)
                    l.Enable(m_data, m_title, m_useLetters, m_selectionDelegate);
                else
                    l.Disable();
            }
        }
    }
}
