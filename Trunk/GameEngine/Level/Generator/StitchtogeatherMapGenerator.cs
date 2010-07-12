using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    // The idea of this clsas is that we read from file(s) a set of 
    // building blocks that we can stitch togeather to get a good map.
    // There are Entrances, hallways (horizontal and vertical), Main Rooms
    // Treausre Rooms, and side rooms
    internal sealed class StitchtogeatherMapGenerator : MapGeneratorBase
    {
        private const int Width = 250;
        private const int Height = 250;

        private static List<MapChunk> m_entrances;
        private static List<MapChunk> m_halls;
        private static List<MapChunk> m_mainRooms;
        private static List<MapChunk> m_treasureRooms;
        private static List<MapChunk> m_sideRooms;
        private static bool m_chunksLoaded = false;

        private StitchtogeatherMapGraphGenerator m_graphGenerator;
        private Queue<MapNode> m_unplacedDueToSpace;
        private int m_smallestX;
        private int m_smallestY;
        private int m_largestX;
        private int m_largestY;
        private int m_placed;

        internal StitchtogeatherMapGenerator(TCODRandom random) : base(random)
        {
            m_unplacedDueToSpace = new Queue<MapNode>();
            m_graphGenerator = new StitchtogeatherMapGraphGenerator();
            m_smallestX = Int32.MaxValue;
            m_smallestY = Int32.MaxValue;
            m_largestX = Int32.MinValue;
            m_largestY = Int32.MinValue;
            m_placed = 0;

            LoadChunksFromFile("Map" + Path.DirectorySeparatorChar + "DungeonChunks.dat");
        }

        // So we don't have to walk the entire map to find the lower left and trim
        // as we generate the map, keep track
        private void PossiblyUpdateLargestSmallestPoint(Point upperLeft, MapChunk chunk)
        {
            m_placed++;
            Point lowerRight = upperLeft + new Point(chunk.Width, chunk.Height);
            m_smallestX = Math.Min(m_smallestX, upperLeft.X);
            m_smallestY = Math.Min(m_smallestY, upperLeft.Y);
            m_largestX = Math.Max(m_largestX, lowerRight.X);
            m_largestY = Math.Max(m_largestY, lowerRight.Y);

            if (m_smallestX < 0)
                throw new ArgumentException("Width too small");
            if (m_smallestY < 0)
                throw new ArgumentException("Height too small");
            if (m_largestX >= Width)
                throw new ArgumentException("Width too large");
            if (m_largestY >= Height)
                throw new ArgumentException("Height too large");
        }

        internal override Map GenerateMap(Stairs incommingStairs)
        {
            Map map = new Map(Width, Height);

            MapNode graphHead = m_graphGenerator.GenerateMapGraph();

            ParenthoodChain parentChain = new ParenthoodChain();
   
            GenerateMapFromGraph(graphHead, map, Point.Invalid, parentChain);

            Point upperLeft = new Point(m_smallestX, m_smallestY);
            Point lowerRight = new Point(m_largestX, m_largestY);

            map.TrimToSubset(upperLeft, lowerRight);

            if (!CheckConnectivity(map))
                throw new MapGenerationFailureException("Generated non-connected map");

            if (m_placed < 30)
                throw new MapGenerationFailureException("Too few items placed to be reasonabily sized");

            GenerateUpDownStairs(map, incommingStairs);

            StripImpossibleDoors(map);

            return map;
        }

        private void StripImpossibleDoors(Map map)
        {
            foreach (MapDoor door in map.MapObjects.OfType<MapDoor>())
            {
                if (!map.IsPointOnMap(door.Position) || map.GetTerrainAt(door.Position) == TerrainType.Wall ||
                    !WallsOnOneSetOfSides(map, door))
                {
                    map.RemoveMapItem(door);
                }
            }

            List<Point> doorPositions = map.MapObjects.OfType<MapDoor>().Select(x => x.Position).ToList();
            foreach (MapDoor door in map.MapObjects.OfType<MapDoor>())
            {
                if (doorPositions.Exists(x => x != door.Position && PointDirectionUtils.NormalDistance(x, door.Position) < 2))
                {
                    map.RemoveMapItem(door);
                    doorPositions.Remove(door.Position);
                }
            }
        }

        private bool WallsOnOneSetOfSides(Map map, MapDoor door)
        {
            return (map.GetTerrainAt(door.Position + new Point(1, 0)) == TerrainType.Wall &&
                     map.GetTerrainAt(door.Position + new Point(-1, 0)) == TerrainType.Wall) ||
                     (map.GetTerrainAt(door.Position + new Point(0, 1)) == TerrainType.Wall &&
                     map.GetTerrainAt(door.Position + new Point(0, -1)) == TerrainType.Wall);
        }

        private MapChunk GetRandomChunkFromList(List<MapChunk> chunk)
        {
            MapChunk newChunk = new MapChunk(chunk[m_random.getInt(0, chunk.Count - 1)]);
            newChunk.Prepare();
            return newChunk;
        }

        private void GenerateMapFromGraph(MapNode current, Map map, Point seam, ParenthoodChain parentChain)
        {
            if (current.Generated)
                return;

            current.Generated = true;
            bool placed = false;

            switch (current.Type)
            {
                case MapNodeType.Entrance:
                {
                    placed = true;
                    MapChunk entranceChunk = GetRandomChunkFromList(m_entrances);

                    // We need to place entrace so it's at our expected location
                    Point randomCenter = new Point(m_random.getInt(100, 150), m_random.getInt(100, 150));
                    Point entraceUpperLeftCorner = randomCenter - entranceChunk.PlayerPosition;

                    parentChain.Push(entranceChunk, entraceUpperLeftCorner, Point.Invalid);

                    entranceChunk.PlaceChunkOnMapAtPosition(map, entraceUpperLeftCorner, m_random);
                    PossiblyUpdateLargestSmallestPoint(entraceUpperLeftCorner, entranceChunk);
                    map.AddMapItem(CoreGameEngine.Instance.MapObjectFactory.CreateMapObject("StairsUp", entranceChunk.PlayerPosition + entraceUpperLeftCorner));

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
                        map.SetTerrainAt(seam, TerrainType.Wall);
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

        private void UnplacePossibleHallwayToNowhere(Map map, ParenthoodChain parentChain)
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
                        m_placed--;
                        top.Chunk.UnplaceChunkOnMapAtPosition(map, top.UpperLeft);
                        seamToFill = top.Seam;
                        top = localChain.Pop();
                    }
                    else
                    {
                        if (seamToFill == Point.Invalid)
                            throw new MapGenerationFailureException("Trying to fill in invalid seam");
                        map.SetTerrainAt(seamToFill, TerrainType.Wall);
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
                map.SetTerrainAt(seam, TerrainType.Wall);
                return false;
            }
            else
            {
                PossiblyUpdateLargestSmallestPoint(placedUpperLeftCorner, mapChunk);
                map.SetTerrainAt(seam, TerrainType.Floor);
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

        private static void LoadChunksFromFile(string fileName)
        {
            if (!m_chunksLoaded)
            {
                m_chunksLoaded = true;

                m_entrances = new List<MapChunk>();
                m_halls = new List<MapChunk>();
                m_mainRooms = new List<MapChunk>();
                m_treasureRooms = new List<MapChunk>();
                m_sideRooms = new List<MapChunk>();

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

                        if (chunkType != "Hall")
                            newChunk.Doors.AddRange(newChunk.Seams);

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
}