using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;
using libtcodWrapper;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal class MapChunk
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Point> Seams { get; set; }
        public List<Point> MonsterSpawns { get; set; }
        public List<Point> TreasureChests { get; set; }
        public List<Point> Cosmetics { get; set; }
        public Point PlayerPosition { get; set; }
        public MapTile[,] MapSegment { get; set; }
        public MapNodeType Type { get; private set; }

        internal MapChunk(int width, int height, string typeString)
        {
            Width = width;
            Height = height;
            Seams = new List<Point>();
            MonsterSpawns = new List<Point>();
            TreasureChests = new List<Point>();
            Cosmetics = new List<Point>();
            PlayerPosition = Point.Invalid;
            Type = (MapNodeType)Enum.Parse(typeof(MapNodeType), typeString);

            MapSegment = new MapTile[Width, Height];
        }

        internal MapChunk(MapChunk chunk)
        {
            Width = chunk.Width;
            Height = chunk.Height;
            Seams = new List<Point>(chunk.Seams);
            MonsterSpawns = new List<Point>(chunk.MonsterSpawns);
            TreasureChests = new List<Point>(chunk.TreasureChests);
            Cosmetics = new List<Point>(chunk.Cosmetics);
            PlayerPosition = chunk.PlayerPosition;
            Type = chunk.Type;

            MapSegment = new MapTile[Width, Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    MapSegment[i, j] = new MapTile(chunk.MapSegment[i, j]);
                }
            }
        }

        // Return UpperLeftCorner placed on if placed. Else Invalid Point
        internal Point PlaceChunkOnMap(Map map, Point seamToFitAgainst)
        {
            foreach (Point s in Seams)
            {
                foreach (Point offset in (new List<Point>() { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) }).OrderBy(x => Guid.NewGuid()))
                {
                    Point upperLeftCorner = seamToFitAgainst + offset - s;

                    // We want to use the offset to see if we'd fit without a problem.
                    if (IsPositionClear(map, upperLeftCorner))
                    {
                        // But we don't want to use it to place, since we want to override the entrace.
                        PlaceChunkOnMapAtPosition(map, upperLeftCorner);
                        Seams.Remove(s);
                        return upperLeftCorner;
                    }
                }
            }
            return Point.Invalid;
        }

        private bool IsPositionClear(Map map, Point upperLeftCorner)
        {
            if (!map.IsPointOnMap(upperLeftCorner))
                return false;
            if (!map.IsPointOnMap(upperLeftCorner + new Point(Width, Height)))
                return false;

            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Point mapPosition = upperLeftCorner + new Point(i, j);
                    if (map[mapPosition].Terrain == TerrainType.Floor)
                        return false;
                }
            }

            return true;
        }

        internal void PlaceChunkOnMapAtPosition(Map map, Point upperLeftCorner)
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Point mapPosition = upperLeftCorner + new Point(i, j);
                    map.GetInternalTile(mapPosition.X, mapPosition.Y).Terrain = MapSegment[i, j].Terrain;
                }
            }

            foreach (Point monsterPosition in MonsterSpawns)
            {
                // TODO: Get monster based on level
                map.AddMonster(CoreGameEngine.Instance.MonsterFactory.CreateMonster("Monster", upperLeftCorner + monsterPosition));
            }

            foreach (Point treasurePosition in TreasureChests)
            {
                // TODO: Handle other map objects
                map.AddMapItem(CoreGameEngine.Instance.MapObjectFactory.CreateMapObject("Treasure Chest", treasurePosition));
            }

            foreach (Point cosmeticPosition in Cosmetics)
            {
                // TODO: Handle cosmetics
            }
        }

        internal void UnplaceChunkOnMapAtPosition(Map map, Point upperLeftCorner)
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Point mapPosition = upperLeftCorner + new Point(i, j);
                    map.GetInternalTile(mapPosition.X, mapPosition.Y).Terrain = TerrainType.Wall;
                }
            }

            foreach (Point monsterPosition in MonsterSpawns)
            {
                // TODO: Get monster based on level
                foreach (ICharacter m in map.Monsters)
                {
                    if (m.Position == (upperLeftCorner + monsterPosition))
                        map.KillMonster((Monster)m);
                }
            }

            foreach (Point treasurePosition in TreasureChests)
            {
                // TODO: Handle other map objects
                foreach (IMapObject o in map.MapObjects)
                {
                    if (o.Position == (upperLeftCorner + treasurePosition))
                        map.RemoveMapItem((MapObject)o);
                }
            }

            foreach (Point cosmeticPosition in Cosmetics)
            {
                // TODO: Handle cosmetics
            }
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public static int NumberOfNeighbors(MapNodeType current)
        {
            switch (current)
            {
                case MapNodeType.Entrance:
                    return 4;
                case MapNodeType.Hall:
                    return 2;
                case MapNodeType.MainRoom:
                case MapNodeType.SideRoom:
                case MapNodeType.TreasureRoom:
                    return 4;
                case MapNodeType.None:
                    return 0;
                case MapNodeType.NoneGivenYet:
                default:
                    throw new ArgumentException("NumberOfNeighbors - No valid number of neighbors");
            }
        }

        internal void ReadSegmentFromFile(StreamReader reader)
        {
            for (int j = 0; j < Height; ++j)
            {
                string tileLine = reader.ReadLine();
                for (int i = 0; i < Width; ++i)
                {
                    MapSegment[i, j] = new MapTile();
                    switch (tileLine[i])
                    {
                        case '#':
                            MapSegment[i, j].Terrain = TerrainType.Wall;
                            break;
                        case '.':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            break;
                        case '^':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            Seams.Add(new Point(i, j));
                            break;
                        case '@':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            if (PlayerPosition != Point.Invalid)
                                throw new InvalidOperationException("Can't have multiple player position on a mapchunk");
                            PlayerPosition = new Point(i, j);
                            break;
                        case 'M':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            MonsterSpawns.Add(new Point(i, j));
                            break;
                        case '+':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            TreasureChests.Add(new Point(i, j));
                            break;
                        case 'C':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            Cosmetics.Add(new Point(i, j));
                            break;
                    }
                }
            }
        }
        
        // When created, maptiles will have too many of some times that get randomized.
        // Prepare will filter some out.
        internal void Prepare()
        {
            if (Seams.Count < NumberOfNeighbors(Type))
                throw new ArgumentException("Not enough Seams for " + Type.ToString());

            TCODRandom random = new TCODRandom();
            while (Seams.Count > NumberOfNeighbors(Type))
            {
                Point removedPoint = Seams[random.GetRandomInt(0, Seams.Count - 1)];
                Seams.Remove(removedPoint);
                MapSegment[removedPoint.X, removedPoint.Y].Terrain = TerrainType.Wall;
            }
        }
    }
}
