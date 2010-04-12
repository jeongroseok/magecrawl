using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    internal abstract class PainterBase : IHandlePainterRequest
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
