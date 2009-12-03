using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class DisableQuitDialog : RequestBase
    {
        public DisableQuitDialog()
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
