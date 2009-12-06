using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal abstract class MapGeneratorBase
    {
        protected TCODRandom m_random;
        private Dictionary<Map, List<Point>> m_clearPointCache;

        internal MapGeneratorBase(TCODRandom random)
        {
            m_random = random;
            m_clearPointCache = new Dictionary<Map, List<Point>>();
        }    

        abstract internal Map GenerateMap(Point stairsUpPosition, out Point stairsDownPosition);

        public Point GetClearPoint(Map map)
        {
            return GetClearPoint(map, Point.Invalid, 0, 0);
        }

        public Point GetClearPoint(Map map, Point center, int distanceToKeepAway)
        {
            return GetClearPoint(map, Point.Invalid, distanceToKeepAway, 0);
        }

        public Point GetClearPoint(Map map, Point center, int distanceToKeepAway, int distanceFromEdges)
        {
            List<Point> clearPointList;
            if (m_clearPointCache.ContainsKey(map))
                clearPointList = m_clearPointCache[map];
            else
                clearPointList = CalculateClearPointList(map);

            // From a randomized order, check each point
            foreach (Point p in clearPointList)
            {
                // First check to make sure we're not too close to center point
                if (center == Point.Invalid || PointDirectionUtils.LatticeDistance(p, center) > distanceToKeepAway)
                {
                    // Next check distance from edges
                    if (distanceFromEdges > 0)
                    {
                        if (p.X > distanceFromEdges && p.X < (map.Width - distanceFromEdges) && p.Y > distanceFromEdges && p.Y < (map.Height - distanceFromEdges))
                        {
                            clearPointList.Remove(p);
                            m_clearPointCache[map] = clearPointList;
                            return p;
                        }
                        else
                            continue;
                    }
                    clearPointList.Remove(p);
                    m_clearPointCache[map] = clearPointList;
                    return p;
                }
            }
            throw new MapGenerationFailureException("Unable to find clear point far enough away from given point.");
        }

        private static List<Point> CalculateClearPointList(Map map)
        {
            List<Point> clearPointList = new List<Point>();

            bool[,] moveabilityGrid = PhysicsEngine.CalculateMoveablePointGrid(map);

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (moveabilityGrid[i, j])
                        clearPointList.Add(new Point(i, j));
                }
            }
            return clearPointList.OrderBy(a => Guid.NewGuid()).ToList();
        }

        protected void ResetScratch(Map map)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    map.SetScratchAt(new Point(i, j), 0);
                }
            }
        }

        protected void FloodFill(Map map, Point p, int scratchValue)
        {
            if (!map.IsPointOnMap(p))
                return;

            if (map.GetTerrainAt(p) == TerrainType.Floor && map.GetScratchAt(p) == 0)
            {
                map.SetScratchAt(p, scratchValue);
                FloodFill(map, p + new Point(1, 0), scratchValue);
                FloodFill(map, p + new Point(-1, 0), scratchValue);
                FloodFill(map, p + new Point(0, 1), scratchValue);
                FloodFill(map, p + new Point(0, -1), scratchValue);
            }       
        }

        // Make sure map has one connected area
        protected bool CheckConnectivity(Map map)
        {
            const int CheckConnectivityScratchValue = 42;

            // Find a clear point
            Point clearPoint = GetFirstClearPoint(map);

            // Flood fill all connected tiles
            FloodFill(map, clearPoint, CheckConnectivityScratchValue);

            // See if any floor tiles don't have our scratch, if so they are not connected
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Point p = new Point(i,j);
                    if (map.GetTerrainAt(p) == TerrainType.Floor && map.GetScratchAt(p) != CheckConnectivityScratchValue)
                        return false;
                }
            }

            return true;
        }

        protected void FillAllSmallerUnconnectedRooms(Map map)
        {
            int currentScratchNumber = FloodFillWithContigiousNumbers(map);

            // Walk each tile, and count up the different groups.
            int[] numberOfTilesWithThatScratch = new int[currentScratchNumber];
            numberOfTilesWithThatScratch.Initialize();

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Point p = new Point(i,j);
                    if (map.GetTerrainAt(p) == TerrainType.Floor)
                        numberOfTilesWithThatScratch[map.GetScratchAt(p)]++;
                }
            }

            if (numberOfTilesWithThatScratch[0] != 0)
                throw new MapGenerationFailureException("Some valid tiles didn't get a scratch during FillAllSmallerRooms.");
            
            // Find the largest collection
            int biggestNumber = 1;
            for (int i = 2; i < currentScratchNumber; ++i)
            {
                if (numberOfTilesWithThatScratch[i] > numberOfTilesWithThatScratch[biggestNumber])
                    biggestNumber = i;
            }

            // Now walk level, and turn every floor tile without that scratch into a wall
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Point p = new Point (i,j);
                    if (map.GetTerrainAt(p) == TerrainType.Floor && map.GetScratchAt(p) != biggestNumber)
                        map.SetTerrainAt(p, TerrainType.Wall);

                    // And reset the scratch while we're here
                    map.SetScratchAt(p, 0);
                }
            }

            if (!CheckConnectivity(map))
                throw new MapGenerationFailureException("FillAllSmallerUnconnectedRooms produced a non-connected map.");
        }

        // Try to flood fill the map. Each seperate contigious spot gets a different scratch number.
        protected int FloodFillWithContigiousNumbers(Map map)
        {
            ResetScratch(map);
            int currentScratchNumber = 1;

            // First we walk the entire map, flood filling each 
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Point p = new Point(i, j);
                    if (map.GetTerrainAt(p) == TerrainType.Floor && map.GetScratchAt(p) == 0)
                    {
                        FloodFill(map, p, currentScratchNumber);
                        currentScratchNumber++;
                    }
                }
            }

            // If we didn't scratch any tiles, the map must be all walls, bail
            if (currentScratchNumber == 1)
                throw new MapGenerationFailureException("FillAllSmallerUnconnectedRooms came to a level with all walls?");
            return currentScratchNumber;
        }

        private Point GetFirstClearPoint(Map map)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Point p = new Point(i,j);
                    if (map.GetTerrainAt(p) == TerrainType.Floor)
                        return p;
                }
            }
            throw new System.ApplicationException("GetFirstClearPoint found no clear points");
        }

        protected Point GetSmallestPoint(Map map)
        {
            int smallestX = map.Width + 1;
            int smallestY = map.Height + 1;

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (map.GetTerrainAt(new Point(i,j)) == TerrainType.Floor)
                    {
                        smallestX = Math.Min(smallestX, i);
                        smallestY = Math.Min(smallestY, j);
                    }
                }
            }

            if (smallestX == (map.Width + 1) || smallestY == (map.Height + 1))
                throw new System.ApplicationException("GetSmallestPoint found no clear points");
            return new Point(smallestX, smallestY);
        }

        protected Point GetLargestPoint(Map map)
        {
            int largestX = -1;
            int largestY = -1;

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (map.GetTerrainAt(new Point(i,j)) == TerrainType.Floor)
                    {
                        largestX = Math.Max(largestX, i);
                        largestY = Math.Max(largestY, j);
                    }
                }
            }

            if (largestX == -1 || largestY == -1)
                throw new System.ApplicationException("GetSmallestPoint found no clear points");
            return new Point(largestX, largestY);
        }

        private static List<Point> surroundingOneSquareList = new List<Point>
        { 
            new Point(-1, -1), new Point(-1, 0), new Point(-1, 1),
            new Point(0, -1), new Point(0, 1), new Point(1, -1), new Point(1, 0), new Point(1, 1), new Point(0, 0)
        };

        private static List<Point> surroundingTwoSquareList = new List<Point> 
        {
            new Point(-1, -2), new Point(0, -2), new Point(1, -2), new Point(-2, -1), new Point(2, -1), new Point(-2, 0), new Point(2, 0), 
            new Point(-2, 1), new Point(2, 1), new Point(-1, 2), new Point(0, 2), new Point(1, 2), new Point(-1, -1), new Point(-1, 0), new Point(-1, 1),
            new Point(0, -1), new Point(0, 1), new Point(1, -1), new Point(1, 0), new Point(1, 1), new Point(0, 0) 
        };

        public static int CountNumberOfSurroundingFloorTilesOneStepAway(Map map, int x, int y)
        {
            return CountNumberOfSurroundingTilesCore(surroundingOneSquareList, map, x, y, TerrainType.Floor);
        }

        public static int CountNumberOfSurroundingFloorTilesTwoStepAway(Map map, int x, int y)
        {
            return CountNumberOfSurroundingTilesCore(surroundingTwoSquareList, map, x, y, TerrainType.Floor);
        }

        public static int CountNumberOfSurroundingWallTilesOneStepAway(Map map, int x, int y)
        {
            return CountNumberOfSurroundingTilesCore(surroundingOneSquareList, map, x, y, TerrainType.Wall);
        }

        public static int CountNumberOfSurroundingWallTilesTwoStepAway(Map map, int x, int y)
        {
            return CountNumberOfSurroundingTilesCore(surroundingTwoSquareList, map, x, y, TerrainType.Wall);
        }

        private static int CountNumberOfSurroundingTilesCore(List<Point> tileList, Map map, int x, int y, TerrainType typeToLookFor)
        {
            int numberOfFloorTileSurrounding = 0;

            foreach (Point p in tileList)
            {
                Point newPoint = new Point(x + p.X, y + p.Y);
                if (map.IsPointOnMap(newPoint))
                {
                    if (map.GetTerrainAt(newPoint) == typeToLookFor)
                        numberOfFloorTileSurrounding++;
                }
            }
            return numberOfFloorTileSurrounding;
        }

        protected void FillEdgesWithWalls(Map map)
        {
            // Fill edges with walls
            for (int i = 0; i < map.Width; ++i)
            {
                map.SetTerrainAt(new Point(i, 0), TerrainType.Wall);
                map.SetTerrainAt(new Point(i, map.Height - 1), TerrainType.Wall);
            }
            for (int j = 0; j < map.Height; ++j)
            {
                map.SetTerrainAt(new Point(0, j), TerrainType.Wall);
                map.SetTerrainAt(new Point(map.Width - 1, j), TerrainType.Wall);
            }
        }

        protected static bool HasValidStairPositioning(Point upStairsPosition, Map map)
        {
            return map.IsPointOnMap(upStairsPosition) && map.GetTerrainAt(upStairsPosition) == TerrainType.Floor;
        }
        
        protected void GenerateUpDownStairs(Map map, Point stairsUpPosition, out Point stairsDownPosition)
        {
            const int DistanceToKeepDownStairsFromUpStairs = 15;
            MapObject upStairs = CoreGameEngine.Instance.MapObjectFactory.CreateMapObject("Stairs Up", stairsUpPosition);
            map.AddMapItem(upStairs);

            stairsDownPosition = GetClearPoint(map, stairsUpPosition, DistanceToKeepDownStairsFromUpStairs, 5);
            MapObject downStairs = CoreGameEngine.Instance.MapObjectFactory.CreateMapObject("Stairs Down", stairsDownPosition);
            map.AddMapItem(downStairs);
        }

        protected void GenerateMonstersAndChests(Map map, Point playerPosition)
        {
            const int DistanceFromPlayerToKeepClear = 5;
            int monsterToGenerate = m_random.GetRandomInt(10, 20);
            for (int i = 0; i < monsterToGenerate; ++i)
            {
                Monster newMonster = CoreGameEngine.Instance.MonsterFactory.CreateMonster("Monster");
                newMonster.Position = GetClearPoint(map, playerPosition, DistanceFromPlayerToKeepClear);
                map.AddMonster(newMonster);
            }

            int treasureToGenerate = m_random.GetRandomInt(3, 6);
            for (int i = 0; i < treasureToGenerate; ++i)
            {
                Point position = GetClearPoint(map, playerPosition, DistanceFromPlayerToKeepClear);
                MapObject treasure = CoreGameEngine.Instance.MapObjectFactory.CreateMapObject("Treasure Chest", position);
                map.AddMapItem(treasure);
            }
        }
    }
}
