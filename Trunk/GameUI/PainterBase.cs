using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    internal abstract class PainterBase
    {
        public abstract void UpdateFromNewData(IGameEngine engine, Point mapUpCorner);
        public abstract void DrawNewFrame(Console screen);
        public abstract void HandleRequest(string request, object data, object data2);
        public abstract void Dispose();
    }
}
