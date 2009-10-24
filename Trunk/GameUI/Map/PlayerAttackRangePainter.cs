using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal sealed class PlayerAttackRangePainter : MapPainterBase
    {
        private Point m_playerPosition;
        private IWeapon m_equipedWeapon;
        private bool m_enabled;
        private Point m_mapUpCorner;
        private int m_mapWidth;
        private int m_mapHeight;

        public PlayerAttackRangePainter()
        {
            m_playerPosition = new Point();
            m_equipedWeapon = null;
            m_enabled = false;
            m_mapUpCorner = new Point();
            m_mapHeight = 0;
            m_mapWidth = 0;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            m_mapUpCorner = mapUpCorner;
            m_playerPosition = engine.Player.Position;
            m_equipedWeapon = engine.Player.CurrentWeapon;
            m_mapHeight = engine.Map.Height;
            m_mapWidth = engine.Map.Width;
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                foreach (Point position in m_equipedWeapon.TargetablePoints(m_playerPosition))
                {
                    Point screenPlacement = new Point(m_mapUpCorner.X + position.X + 1, m_mapUpCorner.Y + position.Y + 1);

                    if (IsDrawableTile(screenPlacement))
                    {
                        Color currentColor = screen.GetCharBackground(screenPlacement.X, screenPlacement.Y);
                        Color newColor = Color.Interpolate(currentColor, TCODColorPresets.DarkYellow, .5f);
                        screen.SetCharBackground(screenPlacement.X, screenPlacement.Y, newColor);
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
