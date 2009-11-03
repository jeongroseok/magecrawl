using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class SelectQuit : RequestBase
    {
        private QuitSelected m_onSelect;

        public SelectQuit(QuitSelected onSelect)
        {
            m_onSelect = onSelect;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            QuitGamePainter q = painter as QuitGamePainter;
            if (q != null)
                q.SelectQuit(m_onSelect);
        }
    }
}
