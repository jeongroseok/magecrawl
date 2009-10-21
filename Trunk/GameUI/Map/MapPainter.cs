using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    public sealed class MapPainter : MapPainterBase
    {
        private Console m_offscreenConsole;

        public MapPainter()
        {
            m_offscreenConsole = RootConsole.GetNewConsole(OffscreenWidth, OffscreenHeight);
        }

        public override void UpdateFromNewData(IGameEngine engine)
        {
            m_offscreenConsole.Clear();

            m_offscreenConsole.DrawFrame(0, 0, MapDrawnWidth + 1, MapDrawnHeight + 1, true, "Map");

            Point mapUpCorner = CalculateMapCorner(engine.Player);

            for (int i = 0; i < engine.Map.Width; ++i)
            {
                for (int j = 0; j < engine.Map.Height; ++j)
                {
                    DrawThing(mapUpCorner, new Point(i, j), m_offscreenConsole, ConvertTerrianToChar(engine.Map[i, j].Terrain));
                }
            }

            foreach (IMapObject obj in engine.Map.MapObjects)
            {
                DrawThing(mapUpCorner, obj.Position, m_offscreenConsole, ConvertMapObjectToChar(obj.Type));
            }

            foreach (ICharacter obj in engine.Map.Monsters)
            {
                DrawThing(mapUpCorner, obj.Position, m_offscreenConsole, 'M');
            }

            m_offscreenConsole.PutChar(ScreenCenter.X + 1, ScreenCenter.Y + 1, '@');
        }

        public override void DrawNewFrame(Console screen)
        {
            m_offscreenConsole.Blit(0, 0, OffscreenWidth, OffscreenHeight, screen, 0, 0);
        }

        public override void Dispose()
        {
            if (m_offscreenConsole != null)
                m_offscreenConsole.Dispose();
            m_offscreenConsole = null;
        }

        public override void HandleRequest(string request, object data)
        {
        }

        private static void DrawThing(Point mapUpCorner, Point position, Console screen, char symbol)
        {
            Point screenPlacement = new Point(mapUpCorner.X + position.X + 1, mapUpCorner.Y + position.Y + 1);

            if (IsDrawableTile(screenPlacement))
            {
                screen.PutChar(screenPlacement.X, screenPlacement.Y, symbol);
            }
        }
    }
}