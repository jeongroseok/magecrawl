using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class DisableDialog : RequestBase
    {
        public DisableDialog()
        {
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            QuitGamePainter q = painter as QuitGamePainter;
            if (q != null)
            {
                q.Disable();
            }
        }
    }
}
