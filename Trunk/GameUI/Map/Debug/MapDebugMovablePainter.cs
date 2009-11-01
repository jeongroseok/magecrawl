using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Debug
{
    internal sealed class MapDebugMovablePainter : MapPainterBase
    {
        private bool m_enabled;
        private int m_width;
        private int m_height;
        private Point m_mapUpCorner;
        private bool[,] m_moveableGrid;
        
        public MapDebugMovablePainter()
        {
            m_enabled = false;
            m_moveableGrid = null;
            m_width = 0;
            m_height = 0;
            m_mapUpCorner = new Point();
        }

        public void SwapDebugMovable(IGameEngine engine)
        {
            m_enabled = !m_enabled;
            CalculateMovability(engine);
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            CalculateMovability(engine);

            m_width = engine.Map.Width;
            m_height = engine.Map.Height;
            m_mapUpCorner = mapUpCorner;
        }

        private void CalculateMovability(IGameEngine engine)
        {
            // This is expensive, so only do if we've going to use it
            if (m_enabled)
                m_moveableGrid = engine.PlayerMoveableToEveryPoint();
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                for (int i = 0; i < m_width; ++i)
                {
                    for (int j = 0; j < m_height; ++j)
                    {
                        Point screenPlacement = new Point(m_mapUpCorner.X + i + 1, m_mapUpCorner.Y + j + 1);

                        if (IsDrawableTile(screenPlacement))
                        {
                            if (m_moveableGrid[i, j])
                                screen.SetCharBackground(screenPlacement.X, screenPlacement.Y, TCODColorPresets.DarkGreen);
                            else
                                screen.SetCharBackground(screenPlacement.X, screenPlacement.Y, TCODColorPresets.DarkRed);
                        }
                    }
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
