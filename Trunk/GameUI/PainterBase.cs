using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    internal abstract class PainterBase : IHandlePainterRequest
    {
        public abstract void UpdateFromNewData(IGameEngine engine, Point mapUpCorner);
        public abstract void DrawNewFrame(Console screen);
        public abstract void Dispose();

        public virtual void HandleRequest(string request, object data, object data2) 
        { 
        }

        public void HandleRequest(RequestBase request)
        {
            request.DoRequest(this);
        }
    }
}
