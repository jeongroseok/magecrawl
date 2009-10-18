using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public static class MapDrawer
    {
        public const int MapDrawnWidth = 51;
        public const int MapDrawnHeight = 43;
        
        public static Point ScreenCenter = new Point((MapDrawnWidth - 1) / 2, (MapDrawnHeight - 2) / 2);

        public static void DrawMap(IPlayer player, IMap map, Console screen)
        {
            Point mapUpCorner = CalculateMapCorner(player);

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    DrawThing(mapUpCorner, new Point(i, j), screen, ConvertTerrianToChar(map[i, j].Terrain));
                }
            }

            foreach (IMapObject obj in map.MapObjects)
            {
                DrawThing(mapUpCorner, obj.Position, screen, ConvertMapObjectToChar(obj.Type));
            }

            foreach (ICharacter obj in map.Monsters)
            {
                DrawThing(mapUpCorner, obj.Position, screen, 'M');
            }

            screen.PutChar(ScreenCenter.X, ScreenCenter.Y, '@');
        }

        private static void DrawThing(Point mapUpCorner, Point position, Console screen, char symbol)
        {
            Point screenPlacement = new Point(mapUpCorner.X + position.X, mapUpCorner.Y + position.Y);

            if (IsDrawableTile(screenPlacement))
            {
                screen.PutChar(screenPlacement.X, screenPlacement.Y, symbol);
            }
        }

        public static void DrawPathable(Console screen, CoreGameEngine engine)
        {
            Point mapUpCorner = CalculateMapCorner(engine.Player);

            for (int i = 0; i < engine.Map.Width; ++i)
             {
                 for (int j = 0; j < engine.Map.Height; ++j)
                 {
                     Point screenPlacement = new Point(mapUpCorner.X + i, mapUpCorner.Y + j);

                     if (IsDrawableTile(screenPlacement))
                     {
                         IList<Point> path = engine.PlayerPathToPoint(new Point(i, j));
                         if (path != null)
                             screen.SetCharBackground(screenPlacement.X, screenPlacement.Y, TCODColorPresets.DarkGreen);
                         else
                             screen.SetCharBackground(screenPlacement.X, screenPlacement.Y, TCODColorPresets.DarkRed);
                     }
                 }
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
            bool xOk = p.X >= 0 && p.X < MapDrawnWidth;
            bool yOk = p.Y >= 0 && p.Y < MapDrawnHeight;
            return xOk && yOk;
        }
    }
}