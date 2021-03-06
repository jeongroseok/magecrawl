using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    internal abstract class PainterBase : IHandlePainterRequest, System.IDisposable
    {
        public abstract void DrawNewFrame(TCODConsole screen);

        public virtual void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
        }

        public virtual void UpdateFromVisibilityData(TileVisibility[,] visibility)
        {
        }

        public virtual void Dispose() 
        {
        }

        public void HandleRequest(RequestBase request)
        {
            request.DoRequest(this);
        }
    }
}
