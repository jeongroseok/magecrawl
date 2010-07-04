using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableQuitDialog : RequestBase
    {
        private QuitReason m_reason;

        public EnableQuitDialog(QuitReason reason)
        {
            m_reason = reason;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            QuitGamePainter q = painter as QuitGamePainter;
            if (q != null)
                q.Enable(m_reason);
        }
    }
}
