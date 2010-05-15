using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ChangeCursorPosition : RequestBase
    {
        private Point m_newPosition;

        public ChangeCursorPosition(Point newPosition)
        {
            m_newPosition = newPosition;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            PaintingCoordinator painterCoordinator = painter as PaintingCoordinator;
            if (painterCoordinator != null)
                painterCoordinator.CursorSpot = m_newPosition;
            
            MapCursorPainter cursorPainter = painter as MapCursorPainter;
            if (cursorPainter != null)
                cursorPainter.NewCursorPosition();
        }
    }
}
