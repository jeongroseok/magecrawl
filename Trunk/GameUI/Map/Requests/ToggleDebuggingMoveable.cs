using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Map.Debug;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ToggleDebuggingMoveable : RequestBase
    {
        private IGameEngine m_engine;

        public ToggleDebuggingMoveable(IGameEngine engine)
        {
            m_engine = engine;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapDebugMovablePainter m = painter as MapDebugMovablePainter;
            if (m != null)
                m.SwapDebugMovable(m_engine);
        }
    }
}
