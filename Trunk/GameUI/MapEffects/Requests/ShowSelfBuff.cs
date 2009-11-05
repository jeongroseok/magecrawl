using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowSelfBuff : RequestBase
    {
        private Point m_position;
        private EffectDone m_doneDelegate;
        private Color m_color;

        public ShowSelfBuff(EffectDone doneDelegate, Point position, Color color)
        {
            m_doneDelegate = doneDelegate;
            m_position = position;
            m_color = color;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
            {
                m.DrawSelfBuff(m_doneDelegate, m_position, m_color);
            }
        }
    }
}
