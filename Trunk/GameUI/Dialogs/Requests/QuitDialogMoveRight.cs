using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class QuitDialogMoveRight : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            QuitGamePainter q = painter as QuitGamePainter;
            if (q != null)
            {
                q.YesSelected = false;
            }
        }
    }
}
