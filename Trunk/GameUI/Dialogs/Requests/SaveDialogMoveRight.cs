using System;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class SaveDialogMoveRight : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            SaveGamePainter q = painter as SaveGamePainter;
            if (q != null)
                q.SaveSelected = false;
        }
    }
}
