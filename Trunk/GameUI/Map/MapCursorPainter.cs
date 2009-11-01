using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal class MapCursorPainter : MapPainterBase
    {
        private bool m_isSelectionCursor;

        public MapCursorPainter()
        {
            m_isSelectionCursor = false;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_isSelectionCursor)
            {
                screen.SetCharBackground(ScreenCenter.X + 1, ScreenCenter.Y + 1, TCODColorPresets.DarkYellow);
            }
        }

        internal override void DisableAllOverlays()
        {
            m_isSelectionCursor = false;
        }

        internal bool MapCursorEnabled
        {
            get { return m_isSelectionCursor; }
            set { m_isSelectionCursor = value; }
        }
    }
}
