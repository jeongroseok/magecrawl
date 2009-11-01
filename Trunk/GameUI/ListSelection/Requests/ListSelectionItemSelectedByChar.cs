
namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class ListSelectionItemSelectedByChar : RequestBase
    {
        private char m_char;
        private ListItemSelected m_onSelect;

        public ListSelectionItemSelectedByChar(char selectChar, ListItemSelected onSelect)
        {
            m_char = selectChar;
            m_onSelect = onSelect;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter l = painter as ListSelectionPainter;
            if (l != null)
            {
                l.SelectionFromChar(m_char, m_onSelect);
            }
        }
    }
}
