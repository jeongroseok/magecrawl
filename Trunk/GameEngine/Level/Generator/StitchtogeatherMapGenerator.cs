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
        private List<MapChunk> m_halls;
        private List<MapChunk> m_mainRooms;
        private List<MapChunk> m_treasureRooms;
        private List<MapChunk> m_sideRooms;
        private Point m_playerPosition;
        private StitchtogeatherMapGraphGenerator m_graphGenerator;
        private Queue<MapNode> m_unplacedDueToSpace;

        internal StitchtogeatherMapGenerator()
        {
            m_entrances = new List<MapChunk>();
            m_halls = new List<MapChunk>();
            m_mainRooms = new List<MapChunk>();
            m_treasureRooms = new List<MapChunk>();
            m_sideRooms = new List<MapChunk>();
            m_unplacedDueToSpace = new Queue<MapNode>();
            m_playerPosition = Point.Invalid;
            m_graphGenerator = new StitchtogeatherMapGraphGenerator();

            LoadChunksFromFile("Map" + Path.DirectorySeparatorChar + "DungeonChunks.dat");
        }

        internal Map GenerateMap(out Point playerPosition)
        {
            int width = 250;
            int height = 250;
            Map map = new Map(width, height);

            // Point center = new Point(m_random.GetRandomInt(100, 150), m_random.GetRandomInt(100, 150));
            Point center = new Point(100, 100);

            MapNode graphHead = m_graphGenerator.GenerateMapGraph();

            ParenthoodChain parentChain = new ParenthoodChain();
   
            GenerateMapFromGraph(graphHead, map, center, parentChain);

            Point upperLeft = GetSmallestPoint(map) - new Point(1, 1);
            Point lowerRight = GetLargestPoint(map) + new Point(1, 1);

            map.TripToSubset(upperLeft, lowerRight);

            playerPosition = m_playerPosition - upperLeft;

            if (!CheckConnectivity(map))
                throw new System.InvalidOperationException("Generated non-connected map");

            return map;
        }

        private MapChunk GetRandomChunkFromList(List<MapChunk> chunk)
        {
            MapChunk newChunk = new MapChunk(chunk[m_random.GetRandomInt(0, chunk.Count - 1)]);
            newChunk.Prepare();
            return newChunk;
        }

        private void GenerateMapFromGraph(MapNode current, Map map, Point seam, ParenthoodChain parentChain)
        {
            if (current.Generated)
                return;

            current.Generated = true;
            bool placed = false;

            System.Console.WriteLine(string.Format("Generating {0} - {1}", current.Type.ToString(), current.UniqueID.ToString()));

            switch (current.Type)
            {
                case MapNodeType.Entrance:
                {
                    placed = true;
                    Point entraceUpperLeftCorner = seam;
                    MapChunk entranceChunk = GetRandomChunkFromList(m_entrances);

                    parentChain.Push(entranceChunk, entraceUpperLeftCorner, Point.Invalid);

                    entranceChunk.PlaceChunkOnMapAtPosition(map, entraceUpperLeftCorner);
                    m_playerPosition = entranceChunk.PlayerPosition + entraceUpperLeftCorner;

                    if (current.Neighbors.Count != entranceChunk.Seams.Count)
                        throw new InvalidOperationException("Number of neighbors should equal number of seams.");
                    WalkNeighbors(current, entranceChunk, map, entraceUpperLeftCorner, parentChain);

                    parentChain.Pop();

                    break;
                }
                case MapNodeType.Hall:
                {
                    for (int i = 0; i < 10; i++)
                    {
                        placed = PlaceMapNode(current, GetRandomChunkFromList(m_halls), map, seam, parentChain);
                        if (placed)
                            break;
                    }
                    break;
                }
                case MapNodeType.None:
                {
                    // If we have nothing, see if we have any orphan nodes to try to place
                    if (m_unplacedDueToSpace.Count > 0)
                    {
                        // Grab the first unplaced node, and try again.
                        MapNode treeToGraphOn = m_unplacedDueToSpace.Dequeue();
                        treeToGraphOn.Generated = false;
                        GenerateMapFromGraph(treeToGraphOn, map, seam, parentChain);
                    }
                    else
                    {
                        map.GetInternalTile(seam).Terrain = TerrainType.Wall;
                        if (current.Neighbors.Count != 0)
                            throw new InvalidOperationException("None Node types should only have no neighbors");
                    }
                    placed = true;
                    break;
                }
                case MapNodeType.MainRoom:
                {
                    placed = PlaceMapNode(current, GetRandomChunkFromList(m_mainRooms), map, seam, parentChain);
                    break;
                }
                case MapNodeType.TreasureRoom:
                {
                    placed = PlaceMapNode(current, GetRandomChunkFromList(m_treasureRooms), map, seam, parentChain);
                    break;
                }
                case MapNodeType.SideRoom:
                {
                    placed = PlaceMapNode(current, GetRandomChunkFromList(m_sideRooms), map, seam, parentChain);
                    break;
                }
                case MapNodeType.NoneGivenYet:
                default:
                    throw new InvalidOperationException("Trying to generate MapNode from invalid node.");
            }
            if (!placed)
            {
                UnplacePossibleHallwayToNowhere(map, parentChain);
                m_unplacedDueToSpace.Enqueue(current);
            }
        }

        private static void UnplacePossibleHallwayToNowhere(Map map, ParenthoodChain parentChain)
        {
            ParenthoodChain localChain = new ParenthoodChain(parentChain);
            ParenthoodElement top = localChain.Pop();
            Point seamToFill = Point.Invalid;
            if (top.Chunk.Type == MapNodeType.Hall)
            {
                do
                {
                    if (top.Chunk.Type == MapNodeType.Hall)
                    {
                        top.Chunk.UnplaceChunkOnMapAtPosition(map, top.UpperLeft);
                        seamToFill = top.Seam;
                        top = localChain.Pop();
                    }
                    else
                    {
                        if (seamToFill == Point.Invalid)
                            throw new System.InvalidOperationException("Trying to fill in invalid seam");
                        map.GetInternalTile(seamToFill).Terrain = TerrainType.Wall;
                        break;
                    }
                }
                while (true);
            }
        }

        // Returns true if placed, false if we walled off seam
        private bool PlaceMapNode(MapNode current, MapChunk mapChunk, Map map, Point seam, ParenthoodChain parentChain)
        {
            Point placedUpperLeftCorner = mapChunk.PlaceChunkOnMap(map, seam);
            if (placedUpperLeftCorner == Point.Invalid)
            {
                map.GetInternalTile(seam).Terrain = TerrainType.Wall;
                return false;
            }
            else
            {
                map.GetInternalTile(seam).Terrain = TerrainType.Floor;
                parentChain.Push(mapChunk, placedUpperLeftCorner, seam);
                WalkNeighbors(current, mapChunk, map, placedUpperLeftCorner, parentChain);
                parentChain.Pop();
                return true;
            }
        }

        private void WalkNeighbors(MapNode currentNode, MapChunk currentChunk, Map map, Point upperLeft, ParenthoodChain parentChain)
        {
            while (currentNode.Neighbors.Count > 0)
            {
                MapNode nextNode = currentNode.Neighbors[0];
                nextNode.RemoveNeighbor(currentNode);
                currentNode.RemoveNeighbor(nextNode);
                Point seam = currentChunk.Seams[0];
                currentChunk.Seams.Remove(seam);
                GenerateMapFromGraph(nextNode, map, seam + upperLeft, parentChain);
            }
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
                    MapChunk newChunk = new MapChunk(width, height, chunkType);
                    newChunk.ReadSegmentFromFile(inputFile);

                    switch (chunkType)
                    {
                        case "Entrance":
                            m_entrances.Add(newChunk);
                            break;
                        case "Hall":
                            m_halls.Add(newChunk);
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