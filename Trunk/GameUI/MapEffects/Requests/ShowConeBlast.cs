using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowConeBlast : RequestBase
    {
        private List<Point> m_points;
        private EffectDone m_doneDelegate;
        private Color m_color;

        public ShowConeBlast(EffectDone doneDelegate, List<Point> points, Color color)
        {
            m_doneDelegate = doneDelegate;
            m_points = points;
            m_color = color;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
            {
                m.DrawConeBlast(m_doneDelegate, m_points, m_color);
            }
        }
    }
}
