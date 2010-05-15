using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class SelectSave : RequestBase
    {
        private SaveSelected m_onSelect;

        public SelectSave(SaveSelected onSelect)
        {
            m_onSelect = onSelect;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            SaveGamePainter q = painter as SaveGamePainter;
            if (q != null)
                q.SelectSave(m_onSelect);
        }
    }
}
