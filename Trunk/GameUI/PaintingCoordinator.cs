using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Inventory;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.Map;
using Magecrawl.GameUI.Map.Debug;
using Magecrawl.Utilities;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.GameUI
{
    public sealed class PaintingCoordinator : System.IDisposable, IHandlePainterRequest
    {
        private List<PainterBase> m_painters;
        private bool m_isSelectionCursor;
        private Point m_cursorSpot;

        public PaintingCoordinator()
        {
            m_isSelectionCursor = false;
            m_cursorSpot = new Point(0, 0);

            m_painters = new List<PainterBase>();

            // The map painter is special since it should go first to draw the base map.
            m_painters.Add(new MapPainter());

            m_painters.Add(new MapDebugMovablePainter());
            m_painters.Add(new MapEffectsPainter());
            m_painters.Add(new MapDebugFOVPainter());
            m_painters.Add(new PlayerTargetingPainter());

            // This should be last of all map painters to block out map
            m_painters.Add(new MapFOVPainter());

            // The cursor painter should be last of all map painters
            m_painters.Add(new MapCursorPainter());

            // This should be after all map painters since we'll draw 'over' the map
            m_painters.Add(new ListSelectionPainter());

            m_painters.Add(new InventoryItemPainter());
        }

        public void UpdateFromNewData(IGameEngine engine)
        {
            foreach (PainterBase p in m_painters)
            {
                p.UpdateFromNewData(engine, CalculateMapCorner(engine));
            }
        }

        public void DrawNewFrame(Console console)
        {
            foreach (PainterBase p in m_painters)
            {
                p.DrawNewFrame(console);
            }
        }

        public void Dispose()
        {
            foreach (PainterBase p in m_painters)
            {
                p.Dispose();
            }
            m_painters = null;
        }

        internal bool MapCursorEnabled
        {
            get { return m_isSelectionCursor; }
            set { m_isSelectionCursor = value; }
        }

        internal Point CursorSpot
        {
            get { return m_cursorSpot; }
            set { m_cursorSpot = value; }
        }

        public void HandleRequest(RequestBase request)
        {
            foreach (PainterBase p in m_painters)
            {
                p.HandleRequest(request);
            }

            // BCL: not really in the spirit of the rest of the request stuff
            if (request is DisableAllOverlays)
            {
                MapCursorEnabled = false;
            }
            else
            {
                IHandlePainterRequestExtensions.HandleRequest(this, request);
            }
        }

        private Point CalculateMapCorner(IGameEngine engine)
        {
            Point centerFocus = m_isSelectionCursor ? m_cursorSpot : engine.Player.Position;
            return new Point(MapPainterBase.ScreenCenter.X - centerFocus.X, MapPainterBase.ScreenCenter.Y - centerFocus.Y);
        }
    }
}
