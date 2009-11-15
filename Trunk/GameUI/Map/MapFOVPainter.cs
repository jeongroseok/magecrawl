using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal class MapFOVPainter : MapPainterBase
    {
        private bool m_enabled;
        private int m_width;
        private int m_height;
        private TileVisibility[,] m_tileVisibility;
        private IGameEngine m_engine;
        private Point m_mapCorner;
        private Point m_cursorPosition;

        public MapFOVPainter()
        {
            m_enabled = true;
            m_width = 0;
            m_height = 0;
            m_tileVisibility = null;
            m_engine = null;
            m_mapCorner = Point.Invalid;
            m_cursorPosition = Point.Invalid;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            if (m_enabled)
            {
                m_engine = engine;
                m_tileVisibility = engine.CalculateTileVisibility();
                m_width = engine.Map.Width;
                m_height = engine.Map.Height;
                m_mapCorner = mapUpCorner;
                m_cursorPosition = cursorPosition;
            }
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                int lowX = m_cursorPosition.X - MapDrawnWidth / 2;
                int lowY = m_cursorPosition.Y - MapDrawnHeight / 2;
                for (int i = lowX; i < lowX + MapDrawnWidth; ++i)
                {
                    for (int j = lowY; j < lowY + MapDrawnHeight; ++j)
                    {
                        int screenPlacementX = m_mapCorner.X + i + 1;
                        int screenPlacementY = m_mapCorner.Y + j + 1;

                        if (IsDrawableTile(screenPlacementX, screenPlacementY))
                        {
                            if (m_engine.Map.IsPointOnMap(new Point(i, j)))
                            {
                                TileVisibility isVisible = m_tileVisibility[i, j];
                                if (isVisible == TileVisibility.Unvisited)
                                {
                                    // If it's unvisisted, nuke the square completed black
                                    screen.SetCharBackground(screenPlacementX, screenPlacementY, TCODColorPresets.Black);
                                    screen.SetCharForeground(screenPlacementX, screenPlacementY, TCODColorPresets.Black);
                                    screen.PutChar(screenPlacementX, screenPlacementY, ' ');
                                }
                                else if (isVisible == TileVisibility.Visited)
                                {
                                    // If it's visited, we're going to trust the rest of the painters to respect visibility to not
                                    // draw non-terrain/maptiles
                                    screen.SetCharBackground(screenPlacementX, screenPlacementY, TCODColorPresets.Black);
                                    screen.SetCharForeground(screenPlacementX, screenPlacementY, Color.FromRGB(40, 40, 40));
                                }
                            }
                        }
                    }
                }
            }
        }

        internal bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }
    }
}
