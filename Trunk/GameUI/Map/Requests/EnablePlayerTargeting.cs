using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public delegate List<Point> PlayerTargettingHaloDelegate(Point p);

    public class EnablePlayerTargeting : RequestBase
    {
        private List<EffectivePoint> m_targetablePoints;
        private bool m_enable;

        public PlayerTargettingHaloDelegate HaloDelegate
        {
            get;
            set;
        }

        public EnablePlayerTargeting(bool enable)
        {
            m_enable = enable;
            m_targetablePoints = null;

            if (m_enable)
                throw new System.ArgumentException("EnablePlayerTargeting(bool enable) must only be called if enable is false");
        }

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
                    p.EnablePlayerTargeting(m_targetablePoints, HaloDelegate);
                else
                    p.DisableAllOverlays();
            }
        }
    }
}
