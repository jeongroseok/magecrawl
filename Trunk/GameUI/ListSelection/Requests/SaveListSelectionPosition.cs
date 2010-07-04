using System;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class SaveListSelectionPosition : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter p = painter as ListSelectionPainter;
            if (p != null)
                p.SaveSelectionPosition = true;
        }
    }
}
