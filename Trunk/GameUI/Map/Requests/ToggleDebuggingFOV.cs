using Magecrawl.GameUI.Map.Debug;
using Magecrawl.Interfaces;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ToggleDebuggingFOV : RequestBase
    {
        private IGameEngine m_engine;

        public ToggleDebuggingFOV(IGameEngine engine)
        {
            m_engine = engine;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapDebugFOVPainter d = painter as MapDebugFOVPainter;
            if (d != null)
                d.SwapDebugFOV(m_engine);
        }
    }
}
