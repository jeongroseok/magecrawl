using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal sealed class PlayerTargetingPainter : MapPainterBase
    {
        private Point m_playerPosition;
        private bool m_enabled;
        private Point m_mapUpCorner;
        private int m_mapWidth;
        private int m_mapHeight;
        private List<EffectivePoint> m_targetablePoints;

        public PlayerTargetingPainter()
        {
            m_playerPosition = new Point();
            m_enabled = false;
            m_mapUpCorner = new Point();
            m_targetablePoints = null;
            m_mapHeight = 0;
            m_mapWidth = 0;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            m_mapUpCorner = mapUpCorner;
            m_playerPosition = engine.Player.Position;
            m_mapHeight = engine.Map.Height;
            m_mapWidth = engine.Map.Width;            
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                foreach (EffectivePoint point in m_targetablePoints)
                {
                    int screenPlacementX = m_mapUpCorner.X + point.Position.X + 1;
                    int screenPlacementY = m_mapUpCorner.Y + point.Position.Y + 1;

                    if (IsDrawableTile(screenPlacementX, screenPlacementY))
                    {
                        Color attackColor = Color.Interpolate(TCODColorPresets.Black, TCODColorPresets.BrightGreen, point.EffectiveStrength);
                        Color currentColor = screen.GetCharBackground(screenPlacementX, screenPlacementY);
                        Color newColor = Color.Interpolate(currentColor, attackColor, .5f);
                        screen.SetCharBackground(screenPlacementX, screenPlacementY, newColor);
                    }
                }
            }
        }

        internal void EnablePlayerTargeting(List<EffectivePoint> targetablePoints)
        {
            m_enabled = true;
            m_targetablePoints = targetablePoints;
        }

        internal override void DisableAllOverlays()
        {
            m_enabled = false;
        }

        public override void Dispose()
        {
        }
    }
}
