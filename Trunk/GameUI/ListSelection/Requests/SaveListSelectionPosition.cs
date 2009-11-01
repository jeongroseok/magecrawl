using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.GameUI.ListSelection.Requests
{
    public class SaveListSelectionPosition : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            ListSelectionPainter p = painter as ListSelectionPainter;
            if (p != null)
            {
                p.SaveSelectionPosition = true;
            }
        }
    }
}
