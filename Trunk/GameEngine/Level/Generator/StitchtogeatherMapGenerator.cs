using System;
using System.Collections.Generic;
using System.IO;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    // The idea of this clsas is that we read from file(s) a set of 
    // building blocks that we can stitch togeather to get a good map.
    // There are Entrances, hallways (horizontal and vertical), Main Rooms
    // Treausre Rooms, and side rooms
    internal sealed class StitchtogeatherMapGenerator : MapGeneratorBase
    {
        private class MapChunk 
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public List<Point> Seams { get; set; }
            public List<Point> MonsterSpawns { get; set; }
            public List<Point> TreasureChests { get; set; }
            public List<Point> Cosmetics { get; set; }
            public Point PlayerPosition { get; set; }
            public MapTile[,] MapSegment { get; set; }

            internal MapChunk(int width, int height)
            {
                Width = width;
                Height = height;
                Seams = new List<Point>();
                MonsterSpawns = new List<Point>();
                TreasureChests = new List<Point>();
                Cosmetics = new List<Point>();
                PlayerPosition = Point.Invalid;

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
                    foreach (Point offset in new List<Point>() { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
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
        }

        private enum MapNodeType
        { 
            NoneGivenYet,
            None,
            Entrance,
            Hall,
            MainRoom,
            TreasureRoom,
            SideRoom
        }

        private class MapNode
        {
            private static int nextUnqiueID = 1;

            public List<MapNode> Neighbors;
            public MapNodeType Type;
            public bool Generated;
            public int Scratch;
            public int UniqueID;

            internal MapNode()
            {
                Neighbors = new List<MapNode>();
                Type = MapNodeType.NoneGivenYet;
                Generated = false;
                Scratch = 0;
                
                UniqueID = nextUnqiueID;
                nextUnqiueID++;
            }
            
            internal MapNode(MapNodeType type)
            {
                Neighbors = new List<MapNode>();
                Type = type;
                Generated = false;
                Scratch = 0;

                UniqueID = nextUnqiueID;
                nextUnqiueID++;
            }

            internal void AddNeighbor(MapNode neighbor)
            {
                Neighbors.Add(neighbor);
            }

            internal void RemoveNeighbor(MapNode neighbor)
            {
                Neighbors.Remove(neighbor);
            }

            public override string ToString()
            {
                return Type.ToString() + " - " + UniqueID.ToString();
            }
        }

        private List<MapChunk> m_entrances;
        private List<MapChunk> m_horizontalHalls;
        private List<MapChunk> m_verticalHalls;
        private List<MapChunk> m_mainRooms;
        private List<MapChunk> m_treasureRooms;
        private List<MapChunk> m_sideRooms;
        private Point m_playerPosition;

        internal StitchtogeatherMapGenerator()
        {
            m_entrances = new List<MapChunk>();
            m_horizontalHalls = new List<MapChunk>();
            m_verticalHalls = new List<MapChunk>();
            m_mainRooms = new List<MapChunk>();
            m_treasureRooms = new List<MapChunk>();
            m_sideRooms = new List<MapChunk>();
            m_playerPosition = Point.Invalid;
            LoadChunksFromFile("Map" + Path.DirectorySeparatorChar + "DungeonChunks.dat");
        }

        internal Map GenerateMap(out Point playerPosition)
        {
            MapNode graphHead = GenerateMapGraph();

            int width = 250;
            int height = 250;
            Map map = new Map(width, height);

            // Point center = new Point(m_random.GetRandomInt(100, 150), m_random.GetRandomInt(100, 150));
            Point center = new Point(100, 100);

            GenerateMapFromGraph(graphHead, map, center);

            playerPosition = m_playerPosition;

            if (!CheckConnectivity(map))
            {
                map.PrintMapToStdOut();
                throw new System.InvalidOperationException("Generated non-connected map");
            }

            return map;
        }

        private void PrintDebugString(string s)
        {
            #if DEBUG
                System.Console.Out.WriteLine(s);
            #endif
        }

        private MapChunk GetRandomChunkFromList(List<MapChunk> chunk)
        {
            return new MapChunk(chunk[m_random.GetRandomInt(0, chunk.Count - 1)]);
        }

        private void GenerateMapFromGraph(MapNode current, Map map, Point seam)
        {
            if (current.Generated)
                return;

            PrintDebugString(string.Format("Generating {0} - {1}", current.Type.ToString(), current.UniqueID.ToString()));

            current.Generated = true;

            switch (current.Type)
            {
                case MapNodeType.Entrance:
                {
                    Point entraceUpperLeftCorner = seam;
                    MapChunk entranceChunk = GetRandomChunkFromList(m_entrances);
                    entranceChunk.PlaceChunkOnMapAtPosition(map, entraceUpperLeftCorner);
                    m_playerPosition = entranceChunk.PlayerPosition + entraceUpperLeftCorner;

                    if (current.Neighbors.Count != entranceChunk.Seams.Count)
                        throw new InvalidOperationException("Number of neighbors should equal number of seams.");
                    WalkNeighbors(current, entranceChunk, map, entraceUpperLeftCorner);
                    break;
                }
                case MapNodeType.Hall:
                {
                    // First see if we're vertical or horizontal
                    bool aboveIsWall = map[seam + new Point(0, -1)].Terrain == TerrainType.Wall;
                    bool belowIsWall = map[seam + new Point(0, 1)].Terrain == TerrainType.Wall;
                    bool leftIsWall = map[seam + new Point(-1, 0)].Terrain == TerrainType.Wall;
                    bool rightIsWall = map[seam + new Point(1, 0)].Terrain == TerrainType.Wall;
                    if (aboveIsWall && belowIsWall)
                    {
                        PlaceMapNode(current,  GetRandomChunkFromList(m_horizontalHalls), map, seam);
                    }
                    else if (leftIsWall && rightIsWall)
                    {
                        PlaceMapNode(current, GetRandomChunkFromList(m_verticalHalls), map, seam);
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Can't find good position for hallway?");
                    }
                    break;
                }
                case MapNodeType.None:
                {
                    map.GetInternalTile(seam.X, seam.Y).Terrain = TerrainType.Wall;
                    if (current.Neighbors.Count != 0)
                        throw new InvalidOperationException("None Node types should only have no neighbors");
                    break;
                }
                case MapNodeType.MainRoom:
                {
                    PlaceMapNode(current, GetRandomChunkFromList(m_mainRooms), map, seam);
                    break;
                }
                case MapNodeType.NoneGivenYet:
                default:
                    throw new InvalidOperationException("Trying to generate MapNode from invalid node.");
            }
        }

        private void PlaceMapNode(MapNode current, MapChunk mapChunk, Map map, Point seam)
        {
            Point placedUpperLeftCorner = mapChunk.PlaceChunkOnMap(map, seam);
            if (placedUpperLeftCorner == Point.Invalid)
            {
                map.GetInternalTile(seam.X, seam.Y).Terrain = TerrainType.Wall;
            }
            else
            {
                WalkNeighbors(current, mapChunk, map, placedUpperLeftCorner);
            }
        }

        private void WalkNeighbors(MapNode currentNode, MapChunk currentChunk, Map map, Point upperLeft)
        {
            while (currentNode.Neighbors.Count > 0)
            {
                MapNode nextNode = currentNode.Neighbors[0];
                nextNode.RemoveNeighbor(currentNode);
                currentNode.RemoveNeighbor(nextNode);
                Point seam = currentChunk.Seams[0];
                currentChunk.Seams.Remove(seam);
                GenerateMapFromGraph(nextNode, map, seam + upperLeft);
            }
        }

        private MapNode GenerateMapGraph()
        {
            int numberOfHallsGenerated = 0;
            int numberOfMainRoomsGenerated = 0;

            MapNode graphHead = new MapNode(MapNodeType.Entrance);

            Queue<MapNode> nodesToHandle = new Queue<MapNode>();
            nodesToHandle.Enqueue(graphHead);

            while (nodesToHandle.Count > 0)
            {
                MapNode currentNode = nodesToHandle.Dequeue();
                switch (currentNode.Type)
                {
                    case MapNodeType.Entrance:
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            if (m_random.Chance(80))
                                GenerateHallway(ref numberOfHallsGenerated, nodesToHandle, currentNode);
                            else
                                GenerateMainroom(ref numberOfMainRoomsGenerated, nodesToHandle, currentNode);
                        }
                        break;
                    }
                    case MapNodeType.Hall:
                    {
                        if (m_random.Chance(50))
                            GenerateMainroom(ref numberOfMainRoomsGenerated, nodesToHandle, currentNode);
                        else
                        {
                            if (m_random.Chance(75))
                                GenerateHallway(ref numberOfHallsGenerated, nodesToHandle, currentNode);
                            else
                                AddNeighborsToNode(MapNodeType.None, currentNode, nodesToHandle);
                        }
                        break;
                    }
                    case MapNodeType.MainRoom:
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            if (m_random.Chance(50))
                                GenerateHallway(ref numberOfHallsGenerated, nodesToHandle, currentNode);
                            else
                                AddNeighborsToNode(MapNodeType.None, currentNode, nodesToHandle);
                        }
                        break;
                    }
                    case MapNodeType.None:
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("{0} should not be placed here.", currentNode.Type.ToString()));
                }
            }

            return graphHead;
        }

        private static void GenerateMainroom(ref int numberOfMainRoomsGenerated, Queue<MapNode> nodesToHandle, MapNode currentNode)
        {
            const int NumberOfMainRoomsToGenerate = 4;
            if (numberOfMainRoomsGenerated < NumberOfMainRoomsToGenerate)
            {
                AddNeighborsToNode(MapNodeType.MainRoom, currentNode, nodesToHandle);
                numberOfMainRoomsGenerated++;
            }
            else
            {
                AddNeighborsToNode(MapNodeType.None, currentNode, nodesToHandle);
            }
        }

        private static void GenerateHallway(ref int numberOfHallsGenerated, Queue<MapNode> nodesToHandle, MapNode currentNode)
        {
            const int NumberOfHallsToGenerate = 20;
            if (numberOfHallsGenerated < NumberOfHallsToGenerate)
            {
                AddNeighborsToNode(MapNodeType.Hall, currentNode, nodesToHandle);
                numberOfHallsGenerated++;
            }
            else
            {
                AddNeighborsToNode(MapNodeType.None, currentNode, nodesToHandle);
            }
        }

        private static void AddNeighborsToNode(MapNodeType type, MapNode parent, Queue<MapNode> nodeQueue)
        {
            MapNode neightbor = new MapNode(type);
            parent.AddNeighbor(neightbor);
            neightbor.AddNeighbor(parent);
            nodeQueue.Enqueue(neightbor);
        }

        private void LoadChunksFromFile(string fileName)
        {
            using (StreamReader inputFile = new StreamReader(fileName))
            {
                while (!inputFile.EndOfStream)
                {
                    string definationLine = inputFile.ReadLine();
                    string[] definationParts = definationLine.Split(' ');
                    int width = int.Parse(definationParts[0]);
                    int height = int.Parse(definationParts[1]);
                   
                    string chunkType = definationParts[2];
                    MapChunk newChunk = new MapChunk(width, height);
                    newChunk.ReadSegmentFromFile(inputFile);

                    switch (chunkType)
                    {
                        case "Entrance":
                            m_entrances.Add(newChunk);
                            break;
                        case "HorizontalHall":
                            m_horizontalHalls.Add(newChunk);
                            break;
                        case "VerticalHall":
                            m_verticalHalls.Add(newChunk);
                            break;
                        case "MainRoom":
                            m_mainRooms.Add(newChunk);
                            break;
                        case "TreasureRoom":
                            m_treasureRooms.Add(newChunk);
                            break;
                        case "SideRoom":
                            m_sideRooms.Add(newChunk);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unknown Chunk Type Read From File");
                    }

                    inputFile.ReadLine();
                }
            }
        }
    }
}