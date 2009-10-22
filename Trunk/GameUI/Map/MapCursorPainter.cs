using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using libtcodWrapper;

namespace Magecrawl.GameUI.Map
{
    public class MapCursorPainter : MapPainterBase
    {
        bool m_isSelectionCursor;
        Point m_cursorSpot;

        public MapCursorPainter()
        {
            m_isSelectionCursor = false;
            m_cursorSpot = new Point(0, 0);
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            m_isSelectionCursor = engine.SelectingTarget;
            m_cursorSpot = engine.TargetSelection;
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_isSelectionCursor)
            {
                screen.SetCharBackground(ScreenCenter.X + 1, ScreenCenter.Y + 1, Color.FromRGB(0x7F, 0xbF, 0));
            }
        }

        public override void HandleRequest(string request, object data)
        {
        }
    }
}
