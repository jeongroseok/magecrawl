using System.Collections.Generic;
using libtcod;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowStream : RequestBase
    {
        private List<Point> m_path;
        private TCODColor m_color;
        Dictionary<Point, bool> m_locationsOccupied;

        public ShowStream(List<Point> path, TCODColor color, Dictionary<Point, bool> locationsOccupied)
        {
            m_path = path;
            m_color = color;
            m_locationsOccupied = locationsOccupied;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
                m.DrawStream(m_path, m_color, m_locationsOccupied);
        }
    }
}
