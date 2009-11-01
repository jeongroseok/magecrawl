using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.GameUI
{
    internal abstract class PainterBase : IHandlePainterRequest
    {
        public abstract void UpdateFromNewData(IGameEngine engine, Point mapUpCorner);
        public abstract void DrawNewFrame(Console screen);
        public abstract void Dispose();

        public virtual void HandleRequest(string request, object data, object data2) { }
        public void HandleRequest(RequestBase request)
        {
            request.DoRequest(this);
        }
    }
}
