using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    public sealed class MapPaintingCoordinator : System.IDisposable
    {
        private List<MapPainterBase> m_painters;
        
        public MapPaintingCoordinator()
        {
            m_painters = new List<MapPainterBase>();

            // The map painter is special since it should go first to draw the base map.
            m_painters.Add(new MapPainter());

            m_painters.Add(new MapDebugMovablePainter());
            m_painters.Add(new MapEffectsPainter());
            m_painters.Add(new MapDebugFOVPainter());
        }

        public void UpdateFromNewData(IGameEngine engine)
        {
            foreach (MapPainterBase p in m_painters)
            {
                p.UpdateFromNewData(engine, CalculateMapCorner(engine.Player));
            }
        }

        public void DrawNewFrame(Console console)
        {
            foreach (MapPainterBase p in m_painters)
            {
                p.DrawNewFrame(console);
            }
        }

        public void Dispose()
        {
            foreach (MapPainterBase p in m_painters)
            {
                p.Dispose();
            }
            m_painters = null;
        }

        public void HandleRequest(string request, object data)
        {
            foreach (MapPainterBase p in m_painters)
            {
                p.HandleRequest(request, data);
            }
        }

        private static Point CalculateMapCorner(IPlayer player)
        {
            return new Point(MapPainterBase.ScreenCenter.X - player.Position.X, MapPainterBase.ScreenCenter.Y - player.Position.Y);
        }
    }
}
