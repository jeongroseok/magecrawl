using System;
using System.Collections.Generic;
using System.Text;

namespace Magecrawl.GameUI.Map.Requests
{
    public class SwapFOVEnabledStatus : RequestBase
    {
        public SwapFOVEnabledStatus()
        {
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapPainter p = painter as MapPainter;
            if (p != null)
            {
                p.HonorFOV = !p.HonorFOV;
            }
        }
    }
}
