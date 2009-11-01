using System;
using System.Collections.Generic;
using System.Text;

namespace Magecrawl.GameUI.Map.Requests
{
    public class DisableAllOverlays : RequestBase
    {
        public DisableAllOverlays()
        {
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapPainterBase m = painter as MapPainterBase;
            if (m != null)
            {
                m.DisableAllOverlays();
            }
        }
    }
}
