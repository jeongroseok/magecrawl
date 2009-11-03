using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class EnableMapCursor : RequestBase
    {
        private bool m_enable;
        private Point m_cursorPosition;

        public EnableMapCursor(bool enable)
        {
            m_enable = enable;
            m_cursorPosition = Point.Invalid;

            if (m_enable)
                throw new System.ArgumentException("EnableMapCursor(bool enable) must only be called if enable is false");
        }

        public EnableMapCursor(bool enable, Point cursorPosition)
        {
            m_enable = enable;
            m_cursorPosition = cursorPosition;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapCursorPainter m = painter as MapCursorPainter;
            PaintingCoordinator c = painter as PaintingCoordinator;
            if (m != null)
            {
                m.MapCursorEnabled = m_enable;
            }
            else if (c != null)
            {
                c.MapCursorEnabled = m_enable;
                c.CursorSpot = m_cursorPosition;
            }
        }
    }
}
