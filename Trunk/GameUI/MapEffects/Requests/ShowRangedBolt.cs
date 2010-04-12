using System.Collections.Generic;
using libtcod;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowRangedBolt : RequestBase
    {
        private List<Point> m_path;
        private EffectDone m_doneDelegate;
        private TCODColor m_color;
        private int m_tailLength;
        private bool m_drawLastTargetSquare;

        public ShowRangedBolt(EffectDone doneDelegate, List<Point> path, TCODColor color, bool drawLastTargetSquare, int tailLength)
        {
            m_doneDelegate = doneDelegate;
            m_path = path;
            m_color = color;
            m_tailLength = tailLength;
            m_drawLastTargetSquare = drawLastTargetSquare;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
                m.DrawRangedBolt(m_doneDelegate, m_path, m_color, m_tailLength, m_drawLastTargetSquare);
        }
    }
}
