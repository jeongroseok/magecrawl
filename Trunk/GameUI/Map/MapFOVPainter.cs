using System.Collections.Generic;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal class MapFOVPainter : MapPainterBase
    {
        private bool m_enabled;
        private TileVisibility[,] m_tileVisibility;
        private IGameEngine m_engine;
        private Point m_mapCorner;
        private Point m_cursorPosition;

        public MapFOVPainter()
        {
            m_enabled = true;
            m_mapCorner = Point.Invalid;
            m_cursorPosition = Point.Invalid;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            if (m_enabled)
            {
                m_engine = engine;
                m_mapCorner = mapUpCorner;
                m_cursorPosition = cursorPosition;
            }
        }

        public override void UpdateFromVisibilityData(TileVisibility[,] visibility)
        {
            m_tileVisibility = visibility;
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            if (m_enabled)
            {
                int lowX = m_cursorPosition.X - (MapDrawnWidth / 2);
                int lowY = m_cursorPosition.Y - (MapDrawnHeight / 2);
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
                                    screen.setCharBackground(screenPlacementX, screenPlacementY, ColorPresets.Black);
                                    screen.setCharForeground(screenPlacementX, screenPlacementY, ColorPresets.Black);
                                    screen.putChar(screenPlacementX, screenPlacementY, ' ');
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
