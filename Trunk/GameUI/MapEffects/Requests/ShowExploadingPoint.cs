using System.Collections.Generic;
using libtcod;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowExploadingPoint : RequestBase
    {
        private List<Point> m_path;
        private List<List<Point>> m_blast;
        private TCODColor m_color;

        public ShowExploadingPoint(List<Point> path, List<List<Point>> blast, TCODColor color)
        {
            m_path = path;
            m_blast = blast;
            m_color = color;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
                m.DrawExploadingPointBlast(m_path, m_blast, m_color);
        }
    }
}
