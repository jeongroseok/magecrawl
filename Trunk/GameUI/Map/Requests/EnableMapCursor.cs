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
            MapCursorPainter mapCursor = painter as MapCursorPainter;
            if (mapCursor != null)
            {
                mapCursor.MapCursorEnabled = m_enable;
            }

            PaintingCoordinator painterCord = painter as PaintingCoordinator;                        
            if (painterCord != null)
            {
                painterCord.MapCursorEnabled = m_enable;
                painterCord.CursorSpot = m_cursorPosition;
            }

            CharacterInfo charInfo = painter as CharacterInfo;
            if (charInfo != null)
            {
                charInfo.MapCursorEnabled = m_enable;
                charInfo.CursorSpot = m_cursorPosition;
            }
        }
    }
}
