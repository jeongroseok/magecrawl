using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using libtcod;
using Magecrawl.Actors;
using Magecrawl.Interfaces;
using Magecrawl.Maps.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.Maps.Generator.Stitch
{
    internal class MapChunk
    {
        private static TCODRandom m_random = new TCODRandom();

        public int Width { get; set; }
        public int Height { get; set; }
        public List<Point> Seams { get; set; }
        public List<Point> TreasureChests { get; set; }
        public List<Point> Doors { get; set; }
        public List<Point> Cosmetics { get; set; }
        public Point PlayerPosition { get; set; }
        public MapTile[,] MapSegment { get; set; }
        public MapNodeType Type { get; private set; }

        internal MapChunk(int width, int height, string typeString)
        {
            Width = width;
            Height = height;
            Seams = new List<Point>();
            TreasureChests = new List<Point>();
            Doors = new List<Point>();
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
            TreasureChests = new List<Point>(chunk.TreasureChests);
            Doors = new List<Point>(chunk.Doors);
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

        private int GetMonsterPlacementPriority()
        {
            switch (Type)
            {
                case MapNodeType.Entrance:
                    return 0;
                case MapNodeType.Hall:
                    return 0;
                case MapNodeType.MainRoom:
                    return 6;
                case MapNodeType.SideRoom:
                    return 4;
                case MapNodeType.TreasureRoom:
                    return 4;
                default:
                    throw new ArgumentException("GetMonsterPlacementPriority on invalid MapChunk type");
            }
        }

        // Return UpperLeftCorner placed on if placed. Else Invalid Point
        internal Point PlaceChunkOnMap(Map map, Point seamToFitAgainst, int level)
        {
            foreach (Point s in Seams)
            {
                foreach (Point offset in (new List<Point>() { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) }).Randomize())
                {
                    Point upperLeftCorner = seamToFitAgainst + offset - s;

                    // We want to use the offset to see if we'd fit without a problem.
                    if (IsPositionClear(map, upperLeftCorner))
                    {
                        // But we don't want to use it to place, since we want to override the entrace.
                        PlaceChunkOnMapAtPosition(map, upperLeftCorner, m_random);
                        MonsterPlacer.PlaceMonster(map, upperLeftCorner, upperLeftCorner + new Point(Width, Height), GetSeamPointsOnMapGrid(upperLeftCorner), GetMonsterPlacementPriority(), level);
                        Seams.Remove(s);
                        return upperLeftCorner;
                    }
                }
            }
            return Point.Invalid;
        }

        private List<Point> GetSeamPointsOnMapGrid(Point upperLeftCorner)
        {
            List<Point> returnList = new List<Point>();
            foreach (Point s in Seams)
                returnList.Add(s + upperLeftCorner);
            return returnList;
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
                    if (map.GetTerrainAt(mapPosition) == TerrainType.Floor)
                        return false;
                }
            }

            return true;
        }

        private const int NormalChanceToPlaceDoorAtSpot = 35;
        private const int TreasureRoomChanceToPlaceDoorAtSpot = 95;

        internal void PlaceChunkOnMapAtPosition(Map map, Point upperLeftCorner, TCODRandom random)
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Point mapPosition = upperLeftCorner + new Point(i, j);
                    map.SetTerrainAt(mapPosition, MapSegment[i, j].Terrain);
                }
            }

            MapObjectFactory mapItemFactory = MapObjectFactory.Instance;

            TreasureChests.ForEach(treasurePosition => map.AddMapItem(mapItemFactory.CreateMapObject("TreasureChest", upperLeftCorner + treasurePosition)));

            int chanceToPlaceDoor = this.Type == MapNodeType.TreasureRoom ? TreasureRoomChanceToPlaceDoorAtSpot : NormalChanceToPlaceDoorAtSpot;
            Doors.Where(x => m_random.Chance(chanceToPlaceDoor)).ToList().ForEach(doorPosition => map.AddMapItem(mapItemFactory.CreateMapObject("MapDoor", upperLeftCorner + doorPosition)));

            Cosmetics.ForEach(cosmeticPosition => map.AddMapItem(mapItemFactory.CreateMapObject("Cosmetic", upperLeftCorner + cosmeticPosition)));
        }

        internal void UnplaceChunkOnMapAtPosition(Map map, Point upperLeftCorner)
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Point mapPosition = upperLeftCorner + new Point(i, j);
                    map.SetTerrainAt(mapPosition, TerrainType.Wall);
                }
            }
            Point lowerRight = upperLeftCorner + new Point(Width, Height);
            foreach (Monster m in map.Monsters.OfType<Monster>().Where(x => x.Position.IsInRange(upperLeftCorner, lowerRight)))
            {
                map.RemoveMonster(m);
            }

            foreach (Point treasurePosition in TreasureChests)
            {
                MapObject objectAtPoint = (MapObject)map.MapObjects.SingleOrDefault(x => x.Position == (upperLeftCorner + treasurePosition));
                if (objectAtPoint != null)
                    map.RemoveMapItem(objectAtPoint);
            }

            foreach (Point doorPosition in Doors)
            {
                MapObject objectAtPoint = (MapObject)map.MapObjects.SingleOrDefault(x => x.Position == (upperLeftCorner + doorPosition));
                if (objectAtPoint != null)
                    map.RemoveMapItem(objectAtPoint);
            }

            foreach (Point cosmeticPosition in Cosmetics)
            {
                MapObject objectAtPoint = (MapObject)map.MapObjects.SingleOrDefault(x => x.Position == (upperLeftCorner + cosmeticPosition));
                if (objectAtPoint != null)
                    map.RemoveMapItem(objectAtPoint);
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
                        case '+':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            TreasureChests.Add(new Point(i, j));
                            break;
                        case 'D':
                            MapSegment[i, j].Terrain = TerrainType.Floor;
                            Doors.Add(new Point(i, j));
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
            while (Seams.Count > NumberOfNeighbors(Type))
            {
                Point removedPoint = Seams[m_random.getInt(0, Seams.Count - 1)];
                Seams.Remove(removedPoint);
                MapSegment[removedPoint.X, removedPoint.Y].Terrain = TerrainType.Wall;
            }
        }
    }
}
