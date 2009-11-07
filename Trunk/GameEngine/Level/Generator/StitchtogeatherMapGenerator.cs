using System;
using System.Collections.Generic;
using System.IO;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;

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

            // Returns seams unfilled
            internal List<Point> PlaceChunkOnMapAtPosition(Map map, Point upperLeftCorner)
            {
                for (int i = 0; i < Width; ++i)
                {
                    for (int j = 0; j < Height; ++j)
                    {
                        Point mapPosition = upperLeftCorner + new Point(i,j);
                        map.GetInternalTile(mapPosition.X, mapPosition.Y).Terrain = MapSegment[i, j].Terrain;
                    }
                }
                
                foreach (Point monsterPosition in MonsterSpawns)
                {
                    // TODO: Get monster based on level
                    map.AddMonster(CoreGameEngine.Instance.MonsterFactory.CreateMonster("Monster", monsterPosition));
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

                return Seams;
            }

            // Returns seams unfilled
            internal void PlaceChunkOnMap(Map map, Point seam)
            {

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
            static int NextUnqiueID = 1;

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
                
                UniqueID = NextUnqiueID;
                NextUnqiueID++;
            }
            
            internal MapNode(MapNodeType type)
            {
                Neighbors = new List<MapNode>();
                Type = type;
                Generated = false;
                Scratch = 0;

                UniqueID = NextUnqiueID;
                NextUnqiueID++;
            }

            internal void AddNeighbor(MapNode neighbor)
            {
                Neighbors.Add(neighbor);
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
            LoadChuncksFromFile("Map" + Path.DirectorySeparatorChar + "DungeonChunks.dat");
        }

        internal Map GenerateMap(out Point playerPosition)
        {
            MapNode graphHead = GenerateMapGraph();

            int width = 200;
            int height = 200;
            Map map = new Map(width, height);

            Point center = new Point(m_random.GetRandomInt(80, 120), m_random.GetRandomInt(80, 120));

            GenerateMapFromGraph(graphHead, map, center);

            ValidateAllNodesAreGenerated(graphHead);

            playerPosition = m_playerPosition;
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
            return chunk[m_random.GetRandomInt(0, chunk.Count - 1)];
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
                    MapChunk entranceChunk = GetRandomChunkFromList(m_entrances);
                    entranceChunk.PlaceChunkOnMapAtPosition(map, seam);
                    m_playerPosition = entranceChunk.PlayerPosition + seam;

                    if(current.Neighbors.Count != entranceChunk.Seams.Count)
                        throw new InvalidOperationException("Number of neighbors should equal number of seams.");
                    for(int i = 0 ; i < current.Neighbors.Count ; ++i)
                        GenerateMapFromGraph(current.Neighbors[i], map, entranceChunk.Seams[i] + seam);
                    break;
                }
                case MapNodeType.None:
                {
                    map.GetInternalTile(seam.X, seam.Y).Terrain = TerrainType.Wall;
                    if (current.Neighbors.Count != 1)
                        throw new InvalidOperationException("None Node types should only have 1 neighbor");
                    break;
                }
                case MapNodeType.NoneGivenYet:
                default:
                    throw new InvalidOperationException("Trying to generate MapNode from invalid node.");
            }

        }


        private void ValidateAllNodesAreGenerated(MapNode current)
        {
            const int ValidateScratchValue = 2;

            if (current.Scratch == ValidateScratchValue)
                return;

            PrintDebugString("Validating - " + current.UniqueID.ToString());

            if (current.Generated != true)
                throw new InvalidOperationException("Not all mapnodes were visited?");
            current.Scratch = ValidateScratchValue;

            foreach (MapNode node in current.Neighbors)
                ValidateAllNodesAreGenerated(node);
        }

        private MapNode GenerateMapGraph()
        {
            MapNode graphHead = new MapNode(MapNodeType.Entrance);

            Queue<MapNode> nodesToHandle = new Queue<MapNode>();
            for(int i = 0 ; i < 4 ; ++i)
            {
                MapNode neightbor = new MapNode();
                graphHead.AddNeighbor(neightbor);
                neightbor.AddNeighbor(graphHead);
                nodesToHandle.Enqueue(neightbor);
            }
            while (nodesToHandle.Count > 0)
            {
                MapNode currentNode = nodesToHandle.Dequeue();
                currentNode.Type = MapNodeType.None;
            }

            return graphHead;
        }

        private void LoadChuncksFromFile(string fileName)
        {
            using (StreamReader inputFile = new StreamReader(fileName))
            {
                while(!inputFile.EndOfStream)
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