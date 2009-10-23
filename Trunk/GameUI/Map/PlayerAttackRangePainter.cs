using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using libtcodWrapper;

namespace Magecrawl.GameUI.Map
{
    internal sealed class PlayerAttackRangePainter : MapPainterBase
    {
        private Point m_playerPosition;
        private int m_range;
        private bool m_enabled;
        private Point m_mapUpCorner;
        private int m_mapWidth;
        private int m_mapHeight;

        public PlayerAttackRangePainter ()
        {
            m_playerPosition = new Point();
            m_range = 0;
            m_enabled = false;
            m_mapUpCorner = new Point();
            m_mapHeight = 0;
            m_mapWidth = 0;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            m_mapUpCorner = mapUpCorner;
            m_playerPosition = engine.Player.Position;
            m_range = engine.Player.RangedAttackDistance;
            m_mapHeight = engine.Map.Height;
            m_mapWidth = engine.Map.Width;
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                for (int i = 0; i < m_mapWidth; ++i)
                {
                    for (int j = 0; j < m_mapHeight; ++j)
                    {
                        bool allowable = (System.Math.Abs(i - m_playerPosition.X) + System.Math.Abs(j - m_playerPosition.Y) <= m_range);
                        if (allowable)
                        {
                            Color currentColor = screen.GetCharBackground(m_mapUpCorner.X + i + 1, m_mapUpCorner.Y + j + 1);
                            Color newColor = Color.Interpolate(currentColor, TCODColorPresets.DarkYellow, .5f);
                            screen.SetCharBackground(m_mapUpCorner.X + i + 1, m_mapUpCorner.Y + j + 1, newColor);
                        }
                    }
                }
            }
        }

        public override void HandleRequest(string request, object data)
        {
            switch (request)
            {
                case "RangedAttackEnabled":
                    m_enabled = true;
                    break;
                case "RangedAttackDisabled":
                    m_enabled = false;
                    break;
            }
        }

        public override void Dispose()
        {
        }
    }
}
