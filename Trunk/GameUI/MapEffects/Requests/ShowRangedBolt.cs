using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowRangedBolt : RequestBase
    {
        private List<Point> m_path;
        private EffectDone m_doneDelegate;
        private Color m_color;

        public ShowRangedBolt(EffectDone doneDelegate, List<Point> path, Color color)
        {
            m_doneDelegate = doneDelegate;
            m_path = path;
            m_color = color;
        }


        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
            {
                m.DrawRangedBolt(m_doneDelegate, m_path, m_color);
            }
        }
    }
}
