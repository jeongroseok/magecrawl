using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using libtcodWrapper;

namespace Magecrawl.GameUI.Map
{
    internal class MapCursorPainter : MapPainterBase
    {
        private bool m_isSelectionCursor;

        public MapCursorPainter()
        {
            m_isSelectionCursor = false;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_isSelectionCursor)
            {
                screen.SetCharBackground(ScreenCenter.X + 1, ScreenCenter.Y + 1, Color.FromRGB(0x7F, 0xbF, 0));
            }
        }

        public override void HandleRequest(string request, object data)
        {
            switch (request)
            {
                case "MapCursorEnabled":
                    m_isSelectionCursor = true;
                    break;
                case "MapCursorDisabled":
                    m_isSelectionCursor = false;
                    break;
            }
        }
    }
}
