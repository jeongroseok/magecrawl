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
        private Point m_mapCorner;

        public MapFOVPainter()
        {
            m_enabled = true;
            m_width = 0;
            m_height = 0;
            m_tileVisibility = null;
            m_mapCorner = Point.Invalid;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            if (m_enabled)
            {
                m_tileVisibility = engine.CalculateTileVisibility();
                m_width = engine.Map.Width;
                m_height = engine.Map.Height;
                m_mapCorner = mapUpCorner;
            }
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                for (int i = 0; i < m_width; ++i)
                {
                    for (int j = 0; j < m_height; ++j)
                    {
                        int screenPlacementX = m_mapCorner.X + i + 1;
                        int screenPlacementY = m_mapCorner.Y + j + 1;

                        if (IsDrawableTile(screenPlacementX, screenPlacementY))
                        {
                            if (m_tileVisibility[i, j] == TileVisibility.Unvisited)
                            {
                                // If it's unvisisted, nuke the square completed black
                                screen.SetCharBackground(screenPlacementX, screenPlacementY, TCODColorPresets.Black);
                                screen.SetCharForeground(screenPlacementX, screenPlacementY, TCODColorPresets.Black);
                                screen.PutChar(screenPlacementX, screenPlacementY, ' ');
                            }
                            else if (m_tileVisibility[i, j] == TileVisibility.Visited)
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

        internal bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }
    }
}
