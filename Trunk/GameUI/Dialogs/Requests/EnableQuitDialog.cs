using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableQuitDialog : RequestBase
    {
        private bool m_enable;

        public EnableQuitDialog(bool enable)
        {
            m_enable = enable;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            QuitGamePainter q = painter as QuitGamePainter;
            if (q != null)
            {
                if (m_enable)
                    q.Enable();
                else
                    q.Disable();
            }
        }
    }
}
