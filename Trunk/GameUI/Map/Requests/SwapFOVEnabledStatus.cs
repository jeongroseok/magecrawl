using System;

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
            MapFOVPainter f = painter as MapFOVPainter;
            if (p != null)
                p.HonorFOV = !p.HonorFOV;
            else if (f != null)
                f.Enabled = !f.Enabled;
        }
    }
}
