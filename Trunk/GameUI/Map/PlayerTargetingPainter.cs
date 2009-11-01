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

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
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
                    Point screenPlacement = new Point(m_mapUpCorner.X + point.Position.X + 1, m_mapUpCorner.Y + point.Position.Y + 1);

                    if (IsDrawableTile(screenPlacement))
                    {
                        Color attackColor = Color.Interpolate(TCODColorPresets.Black, TCODColorPresets.BrightGreen, point.EffectiveStrength);
                        Color currentColor = screen.GetCharBackground(screenPlacement.X, screenPlacement.Y);
                        Color newColor = Color.Interpolate(currentColor, attackColor, .5f);
                        screen.SetCharBackground(screenPlacement.X, screenPlacement.Y, newColor);
                    }
                }
            }
        }

        public override void HandleRequest(string request, object data, object data2)
        {
            switch (request)
            {
                case "PlayerTargettingEnabled":
                    m_enabled = true;
                    m_targetablePoints = (List<EffectivePoint>)data;
                    break;
                case "PlayerTargettingDisabled":
                case "DisableAllOverlays":
                    m_enabled = false;
                    break;
            }
        }

        public override void Dispose()
        {
        }
    }
}
