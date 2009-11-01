using System;
using System.Collections.Generic;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class EnableMapCursor : RequestBase
    {
        private bool m_enable;
        private Point m_cursorPosition;

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
