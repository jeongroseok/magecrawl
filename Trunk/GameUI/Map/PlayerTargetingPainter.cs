using System.Collections.Generic;
using libtcod;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.GameUI.Utilities;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal sealed class PlayerTargetingPainter : MapPainterBase
    {
        private bool m_enabled;
        private Point m_mapUpCorner;
        private Point m_cursorPosition;
        private List<EffectivePoint> m_targetablePoints;
        private PlayerTargettingHaloDelegate m_haloDelegate;
        private IMap m_map;

        public PlayerTargetingPainter()
        {
            m_enabled = false;
            m_mapUpCorner = new Point();
            m_targetablePoints = null;
            m_map = null;
            m_cursorPosition = Point.Invalid;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            m_mapUpCorner = mapUpCorner;
            m_cursorPosition = cursorPosition;
            m_map = engine.Map;
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            if (m_enabled)
            {
                foreach (EffectivePoint point in m_targetablePoints)
                {
                    if (m_map.IsPointOnMap(point.Position))
                        ColorSquare(screen, point.Position, point.EffectiveStrength, ColorPresetsFromTCOD.BrightGreen.Divide(2)); 
                }

                if (m_haloDelegate != null)
                {
                    List<Point> pointsToPaint = m_haloDelegate(m_cursorPosition);
                    if (pointsToPaint != null)
                    {
                        foreach (Point p in pointsToPaint)
                        {
                            if (m_map.IsPointOnMap(p))
                                ColorSquare(screen, p, .75, ColorPresets.Yellow);
                        }
                    }
                }
            }
        }

        private void ColorSquare(TCODConsole screen, Point p, double strength, TCODColor color)
        {
            int screenPlacementX = m_mapUpCorner.X + p.X + 1;
            int screenPlacementY = m_mapUpCorner.Y + p.Y + 1;

            if (IsDrawableTile(screenPlacementX, screenPlacementY))
            {
                TCODColor attackColor = TCODColor.Interpolate(ColorPresets.Black, color, (float)strength);
                TCODColor currentColor = screen.getCharBackground(screenPlacementX, screenPlacementY);
                TCODColor newColor = TCODColor.Interpolate(currentColor, attackColor, .5f);
                screen.setCharBackground(screenPlacementX, screenPlacementY, newColor);
            }
        }

        internal void EnablePlayerTargeting(List<EffectivePoint> targetablePoints, PlayerTargettingHaloDelegate haloDelegate)
        {
            m_enabled = true;
            m_targetablePoints = targetablePoints;
            m_haloDelegate = haloDelegate;
        }

        internal override void DisableAllOverlays()
        {
            m_enabled = false;
        }
    }
}
