using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class EnableToolTips : RequestBase
    {
        private bool m_enable;

        public EnableToolTips(bool enable)
        {
            m_enable = enable;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapCursorPainter m = painter as MapCursorPainter;
            if (m != null)
            {
                m.ToolTipsEnabled = m_enable;
            }
        }
    }
}
