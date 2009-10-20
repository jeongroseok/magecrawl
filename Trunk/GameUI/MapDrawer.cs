using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public class MapDrawer : System.IDisposable
    {
        public const int MapDrawnWidth = 50;
        public const int MapDrawnHeight = 42;
        public static Point ScreenCenter = new Point((MapDrawnWidth - 1) / 2, (MapDrawnHeight - 2) / 2);

        private const int OffscreenWidth = MapDrawnWidth + 2;
        private const int OffscreenHeight = MapDrawnHeight + 2;

        private Console m_offscreenConsole;
        private bool m_pathable;
        private IGameEngine m_gameEngine;
        
        public MapDrawer()
        {
            m_offscreenConsole = RootConsole.GetNewConsole(OffscreenWidth, OffscreenHeight);
            m_pathable = false;
            m_gameEngine = null;
        }

        public void Dispose()
        {
            if (m_offscreenConsole != null)
                m_offscreenConsole.Dispose();
            m_offscreenConsole = null;
        }

        public void SwapPathableMode(IGameEngine engine)
        {
            m_gameEngine = engine;
            m_pathable = !m_pathable;
            UpdateMap(engine.Player, engine.Map);
        }

        public void UpdateMap(IPlayer player, IMap map)
        {
            m_offscreenConsole.Clear();
            DrawMapFrame();

            Point mapUpCorner = CalculateMapCorner(player);

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    DrawThing(mapUpCorner, new Point(i, j), m_offscreenConsole, ConvertTerrianToChar(map[i, j].Terrain));
                }
            }

            foreach (IMapObject obj in map.MapObjects)
            {
                DrawThing(mapUpCorner, obj.Position, m_offscreenConsole, ConvertMapObjectToChar(obj.Type));
            }

            foreach (ICharacter obj in map.Monsters)
            {
                DrawThing(mapUpCorner, obj.Position, m_offscreenConsole, 'M');
            }

            m_offscreenConsole.PutChar(ScreenCenter.X + 1, ScreenCenter.Y + 1, '@');

            if (m_pathable)
                DrawPathable();
        }

        public void DrawMap(Console screen)
        {
            m_offscreenConsole.Blit(0, 0, OffscreenWidth, OffscreenHeight, screen, 0, 0);
        }

        private void DrawMapFrame()
        {
            m_offscreenConsole.DrawFrame(0, 0, MapDrawnWidth + 1, MapDrawnHeight + 1, true, "Map");
        }

        private void DrawPathable()
        {
            Point mapUpCorner = CalculateMapCorner(m_gameEngine.Player);

            for (int i = 0; i < m_gameEngine.Map.Width; ++i)
            {
                for (int j = 0; j < m_gameEngine.Map.Height; ++j)
                {
                    Point screenPlacement = new Point(mapUpCorner.X + i + 1, mapUpCorner.Y + j + 1);

                    if (IsDrawableTile(screenPlacement))
                    {
                        IList<Point> path = m_gameEngine.PlayerPathToPoint(new Point(i, j));
                        if (path != null)
                            m_offscreenConsole.SetCharBackground(screenPlacement.X, screenPlacement.Y, TCODColorPresets.DarkGreen);
                        else
                            m_offscreenConsole.SetCharBackground(screenPlacement.X, screenPlacement.Y, TCODColorPresets.DarkRed);
                    }
                }
            }
        }

        private static void DrawThing(Point mapUpCorner, Point position, Console screen, char symbol)
        {
            Point screenPlacement = new Point(mapUpCorner.X + position.X + 1, mapUpCorner.Y + position.Y + 1);

            if (IsDrawableTile(screenPlacement))
            {
                screen.PutChar(screenPlacement.X, screenPlacement.Y, symbol);
            }
        }

        private static char ConvertMapObjectToChar(MapObjectType t)
        {
            switch (t)
            {
                case MapObjectType.OpenDoor:
                    return ';';
                case MapObjectType.ClosedDoor:
                    return ':';
                default:
                    throw new System.ArgumentException("Unknown Type - ConvertMapObjectToChar");
            }
        }

        private static char ConvertTerrianToChar(TerrainType t)
        {
            switch (t)
            {
                case TerrainType.Floor:
                    return ' ';
                case TerrainType.Wall:
                    return '#';
                default:
                    throw new System.ArgumentException("Unknown Type - ConvertTerrianToChar");
            }
        }
        
        private static Point CalculateMapCorner(IPlayer player)
        {
            return new Point(ScreenCenter.X - player.Position.X, ScreenCenter.Y - player.Position.Y);
        }

        private static bool IsDrawableTile(Point p)
        {
            bool xOk = p.X >= 1 && p.X < MapDrawnWidth;
            bool yOk = p.Y >= 1 && p.Y < MapDrawnHeight;
            return xOk && yOk;
        }
    }
}