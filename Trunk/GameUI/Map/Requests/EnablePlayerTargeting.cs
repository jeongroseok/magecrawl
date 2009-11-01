using System;
using System.Collections.Generic;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class EnablePlayerTargeting : RequestBase
    {
        private List<EffectivePoint> m_targetablePoints;
        private bool m_enable;

        public EnablePlayerTargeting(bool enable, List<EffectivePoint> targetablePoints)
        {
            m_enable = enable;
            m_targetablePoints = targetablePoints;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            PlayerTargetingPainter p = painter as PlayerTargetingPainter;
            if (p != null)
            {
                if (m_enable)
                {
                    p.EnablePlayerTargeting(m_targetablePoints);
                }
                else
                {
                    p.DisableAllOverlays();
                }
            }
        }
    }
}
