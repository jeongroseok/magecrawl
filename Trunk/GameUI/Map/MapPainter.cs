using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;
using System.Xml;
using System.IO;

namespace Magecrawl.GameUI.Map
{
    internal sealed class MapPainter : MapPainterBase
    {
        private Console m_offscreenConsole;
        private bool m_honorFOV;
        private TileVisibility[,] m_tileVisibility;
        private Dictionary<string, char> m_monsterSymbols;

        public MapPainter()
        {
            m_offscreenConsole = RootConsole.GetNewConsole(OffscreenWidth, OffscreenHeight);
            m_offscreenConsole.ForegroundColor = UIHelper.ForegroundColor;
            m_honorFOV = true;
            LoadMonsterSymbols();
        }

        internal bool HonorFOV
        {
            get { return m_honorFOV; }
            set { m_honorFOV = value; }
        }

        public override void UpdateFromVisibilityData(TileVisibility[,] visibility)
        {
            m_tileVisibility = visibility;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            TileVisibility[,] tileVisibility = engine.CalculateTileVisibility();

            m_offscreenConsole.Clear();

            m_offscreenConsole.DrawFrame(0, 0, MapDrawnWidth + 1, MapDrawnHeight + 1, true, "Map");

            int lowX = cursorPosition.X - (MapDrawnWidth / 2);
            int lowY = cursorPosition.Y - (MapDrawnHeight / 2);
            for (int i = lowX; i < lowX + MapDrawnWidth; ++i)
            {
                for (int j = lowY; j < lowY + MapDrawnHeight; ++j)
                {
                    Point p = new Point(i, j);
                    if (engine.Map.IsPointOnMap(p))
                    {
                        TerrainType t = engine.Map.GetTerrainAt(p);
                        Color c = ConvertTerrainSpotToColor(t, m_tileVisibility[i, j]);
                        DrawThing(mapUpCorner, p, m_offscreenConsole, c);
                    }
                }
            }

            foreach (IMapObject obj in engine.Map.MapObjects)
            {
                DrawThing(mapUpCorner, obj.Position, m_offscreenConsole, ConvertMapObjectToChar(obj.Type));
            }

            foreach (Pair<IItem, Point> obj in engine.Map.Items)
            {
                TileVisibility visibility = tileVisibility[obj.Second.X, obj.Second.Y];
                if (!m_honorFOV || visibility == TileVisibility.Visible)
                {
                    // If you change this, update HelpPainter.cs
                    DrawThingIfMultipleSpecialSymbol(mapUpCorner, obj.Second, m_offscreenConsole, '&', '%');
                }
            }

            foreach (ICharacter m in engine.Map.Monsters)
            {
                TileVisibility visibility = tileVisibility[m.Position.X, m.Position.Y];
                if (!m_honorFOV || visibility == TileVisibility.Visible)
                    DrawThing(mapUpCorner, m.Position, m_offscreenConsole, m_monsterSymbols[m.Name]);
            }

            DrawThing(mapUpCorner, engine.Player.Position, m_offscreenConsole, '@');
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

        private static void DrawThing(Point mapUpCorner, Point position, Console screen, Color c)
        {
            int screenPlacementX = mapUpCorner.X + position.X + 1;
            int screenPlacementY = mapUpCorner.Y + position.Y + 1;

            if (IsDrawableTile(screenPlacementX, screenPlacementY))
                screen.SetCharBackground(screenPlacementX, screenPlacementY, c);
        }

        private static void DrawThing(Point mapUpCorner, Point position, Console screen, char symbol)
        {
            int screenPlacementX = mapUpCorner.X + position.X + 1;
            int screenPlacementY = mapUpCorner.Y + position.Y + 1;

            if (IsDrawableTile(screenPlacementX, screenPlacementY))
                screen.PutChar(screenPlacementX, screenPlacementY, symbol, Background.None);
        }

        private static void DrawThingIfMultipleSpecialSymbol(Point mapUpCorner, Point position, Console screen, char symbol, char multipleSymbol)
        {
            int screenPlacementX = mapUpCorner.X + position.X + 1;
            int screenPlacementY = mapUpCorner.Y + position.Y + 1;

            if (IsDrawableTile(screenPlacementX, screenPlacementY))
            {
                char currentChar = screen.GetChar(screenPlacementX, screenPlacementY);

                // If we already have one of those, or the multipleSymbol, draw the multipleSymbole, else draw normal.
                if (currentChar == symbol || currentChar == multipleSymbol)
                    screen.PutChar(screenPlacementX, screenPlacementY, multipleSymbol, Background.None);
                else
                    screen.PutChar(screenPlacementX, screenPlacementY, symbol, Background.None);
            }
        }

        // If you change this, update HelpPainter.cs
        private static char ConvertMapObjectToChar(MapObjectType t)
        {
            switch (t)
            {
                case MapObjectType.OpenDoor:
                    return ';';
                case MapObjectType.ClosedDoor:
                    return ':';
                case MapObjectType.TreasureChest:
                    return '+';
                case MapObjectType.Cosmetic:
                    return '_';
                case MapObjectType.StairsDown:
                    return '>';
                case MapObjectType.StairsUp:
                    return '<';
                default:
                    throw new System.ArgumentException("Unknown Type - ConvertMapObjectToChar");
            }
        }

        // If you change this, update HelpPainter.cs
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

        private Color ConvertTerrainSpotToColor(TerrainType terrain, TileVisibility visibility)
        {
            if (m_honorFOV && visibility == TileVisibility.Unvisited)
                return ColorPresets.Black;
            bool visible = visibility == TileVisibility.Visible;
            switch (terrain)
            {
                case TerrainType.Floor:
                    if (visible)
                        return (Color)Preferences.Instance["FloorColorVisible"];
                    else
                        return (Color)Preferences.Instance["FloorColorNotVisible"];
                case TerrainType.Wall:
                    if (visible)
                        return (Color)Preferences.Instance["WallColorVisible"];
                    else
                        return (Color)Preferences.Instance["WallColorNotVisible"];
                default:
                    throw new System.ArgumentException("Unknown Type - ConvertTerrianToChar");
            }
        }

        private void LoadMonsterSymbols()
        {
            m_monsterSymbols = new Dictionary<string, char>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Monsters.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Monsters")
            {
                throw new System.InvalidOperationException("Bad monsters file");
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Monsters")
                    break;

                if (reader.LocalName == "Monster")
                {
                    string name = reader.GetAttribute("Name");
                    char symbol = reader.GetAttribute("Symbol")[0];

                    m_monsterSymbols.Add(name, symbol);
                }
            }
            reader.Close();
        }
    }
}