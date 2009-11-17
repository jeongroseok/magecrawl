using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private List<MapChunk> m_entrances;
        private List<MapChunk> m_horizontalHalls;
        private List<MapChunk> m_verticalHalls;
        private List<MapChunk> m_mainRooms;
        private List<MapChunk> m_treasureRooms;
        private List<MapChunk> m_sideRooms;
        private Point m_playerPosition;
        private StitchRatio m_stitchRatio;

        internal StitchtogeatherMapGenerator()
        {
            m_entrances = new List<MapChunk>();
            m_horizontalHalls = new List<MapChunk>();
            m_verticalHalls = new List<MapChunk>();
            m_mainRooms = new List<MapChunk>();
            m_treasureRooms = new List<MapChunk>();
            m_sideRooms = new List<MapChunk>();
            m_playerPosition = Point.Invalid;
            m_stitchRatio = new StitchRatio(100);

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

        private int NumberOfNeighborsToGenerate(MapNodeType current)
        {
            switch (current)
            {
                case MapNodeType.Entrance:
                    return 4;
                case MapNodeType.Hall:
                    return 1;
                case MapNodeType.MainRoom:
                case MapNodeType.SideRoom:
                case MapNodeType.TreasureRoom:
                    return 3;
                case MapNodeType.None:
                    return 0;
                case MapNodeType.NoneGivenYet:
                default:
                    throw new ArgumentException("NumberOfNeighborsToGenerate - No valid number of neighbors");
            }
        }

        private MapNode GenerateMapGraph()
        {
            MapNode graphHead = new MapNode(MapNodeType.Entrance);

            Queue<MapNode> nodesToHandle = new Queue<MapNode>();
            nodesToHandle.Enqueue(graphHead);

            while (nodesToHandle.Count > 0)
            {
                MapNode currentNode = nodesToHandle.Dequeue();

                for (int i = 0; i < NumberOfNeighborsToGenerate(currentNode.Type); ++i)
                {
                    MapNodeType nodeTypeToGenerate = m_stitchRatio.GetNewNode(currentNode.Type);
                    AddNeighborsToNode(currentNode, nodeTypeToGenerate, nodesToHandle);
                }
            }

            return graphHead;
        }

        private static void AddNeighborsToNode(MapNode parent, MapNodeType type, Queue<MapNode> nodeQueue)
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