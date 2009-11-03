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
            PaintingCoordinator c = painter as PaintingCoordinator;
            if (c != null)
                c.CursorSpot = m_newPosition;
        }
    }
}
