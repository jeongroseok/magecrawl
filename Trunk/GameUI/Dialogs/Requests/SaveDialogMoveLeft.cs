using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class SaveDialogMoveLeft : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            SaveGamePainter q = painter as SaveGamePainter;
            if (q != null)
            {
                q.SaveSelected = q.SaveEnabled || q.SaveSelected;
            }
        }
    }
}
