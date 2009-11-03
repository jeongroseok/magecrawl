using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class QuitDialogMoveLeft : RequestBase
    {        
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            QuitGamePainter q = painter as QuitGamePainter;
            if (q != null)
                q.YesSelected = q.YesEnabled || q.YesSelected;
        }
    }
}
