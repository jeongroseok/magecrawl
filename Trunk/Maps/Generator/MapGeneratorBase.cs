using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Maps.MapObjects;
using Magecrawl.Utilities;
using Magecrawl.EngineInterfaces;

namespace Magecrawl.Maps.Generator
{
    public abstract class MapGeneratorBase
    {
        protected TCODRandom m_random;
        private Dictionary<Map, List<Point>> m_clearPointCache;

        internal MapGeneratorBase(TCODRandom random)
        {
            m_random = random;
            m_clearPointCache = new Dictionary<Map, List<Point>>();
        }

        abstract public Map GenerateMap(Stairs incommingStairs, int level);

        protected Point GetClearPoint(Map map)
        {
            return GetClearPoint(map, Point.Invalid, 0, 0);
        }

        protected Point GetClearPoint(Map map, Point center, int distanceToKeepAway, int distanceFromEdges)
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

        public static List<Point> GetClearPointListInRange(Map map, Point upperLeft, Point lowerRight)
        {
            List<Point> returnList = new List<Point>();

            for (int i = upperLeft.X; i < lowerRight.X; ++i)
            {
                for (int j = upperLeft.Y; j < lowerRight.Y; ++j)
                {
                    if (map.GetTerrainAt(i, j) == TerrainType.Floor)
                        returnList.Add(new Point(i, j));
                }
            }
            return returnList;
        }

        protected static List<Point> CalculateClearPointList(Map map)
        {
            List<Point> clearPointList = new List<Point>();

            bool[,] moveabilityGrid = map.CalculateMoveablePointGrid(true);

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (moveabilityGrid[i, j])
                        clearPointList.Add(new Point(i, j));
                }
            }
            return clearPointList.Randomize();
        }

        protected void ResetScratch(Map map)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    map.SetScratchAt(i, j, 0);
                }
            }
        }

        protected void FloodFill(Map map, int x, int y, byte scratchValue)
        {
            if (!map.IsPointOnMap(x, y))
                return;

            if (map.GetTerrainAt(x, y) == TerrainType.Floor && map.GetScratchAt(x, y) == 0)
            {
                map.SetScratchAt(x, y, scratchValue);
                FloodFill(map, x + 1, y, scratchValue);
                FloodFill(map, x - 1, y, scratchValue);
                FloodFill(map, x, y + 1, scratchValue);
                FloodFill(map, x, y - 1, scratchValue);
            }       
        }

        // Make sure map has one connected area
        protected bool CheckConnectivity(Map map)
        {
            const int CheckConnectivityScratchValue = 42;

            // Find a clear point
            Point clearPoint = GetFirstClearPoint(map);

            // Flood fill all connected tiles
            FloodFill(map, clearPoint.X, clearPoint.Y, CheckConnectivityScratchValue);

            // See if any floor tiles don't have our scratch, if so they are not connected
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (map.GetTerrainAt(i, j) == TerrainType.Floor && map.GetScratchAt(i, j) != CheckConnectivityScratchValue)
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
                    if (map.GetTerrainAt(i, j) == TerrainType.Floor)
                        numberOfTilesWithThatScratch[map.GetScratchAt(i, j)]++;
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
                    if (map.GetTerrainAt(i, j) == TerrainType.Floor && map.GetScratchAt(i, j) != biggestNumber)
                        map.SetTerrainAt(i, j, TerrainType.Wall);

                    // And reset the scratch while we're here
                    map.SetScratchAt(i, j, 0);
                }
            }

            if (!CheckConnectivity(map))
                throw new MapGenerationFailureException("FillAllSmallerUnconnectedRooms produced a non-connected map.");
        }

        // Try to flood fill the map. Each seperate contigious spot gets a different scratch number.
        protected int FloodFillWithContigiousNumbers(Map map)
        {
            ResetScratch(map);
            byte currentScratchNumber = 1;

            // First we walk the entire map, flood filling each 
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (map.GetTerrainAt(i, j) == TerrainType.Floor && map.GetScratchAt(i, j) == 0)
                    {
                        FloodFill(map, i, j, currentScratchNumber);
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
                    if (map.GetTerrainAt(i, j) == TerrainType.Floor)
                        return new Point(i, j);
                }
            }
            throw new System.ApplicationException("GetFirstClearPoint found no clear points");
        }

        protected static int CountNumberOfSurroundingWallTilesOneStepAway(Map map, int x, int y)
        {
            int numberOfFloorTileSurrounding = 0;

            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (map.IsPointOnMap(x + i, y + j))
                    {
                        if (map.GetTerrainAt(x + i, y + j) == TerrainType.Wall)
                            numberOfFloorTileSurrounding++;
                    }
                }
            }
            return numberOfFloorTileSurrounding;
        }

        protected static int CountNumberOfSurroundingWallTilesTwoStepAway(Map map, int x, int y)
        {
            int numberOfFloorTileSurrounding = 0;

            for (int i = -2; i <= 2; ++i)
            {
                for (int j = -2; j <= 2; ++j)
                {
                    if ((i == 2 || i == -2) && (j == 2 || j == -2))
                        continue;

                    if (map.IsPointOnMap(x + i, y + j))
                    {
                        if (map.GetTerrainAt(x + i, y + j) == TerrainType.Wall)
                            numberOfFloorTileSurrounding++;
                    }
                }
            }

            return numberOfFloorTileSurrounding;
        }

        protected void FillEdgesWithWalls(Map map)
        {
            // Fill edges with walls
            for (int i = 0; i < map.Width; ++i)
            {
                map.SetTerrainAt(i, 0, TerrainType.Wall);
                map.SetTerrainAt(i, map.Height - 1, TerrainType.Wall);
            }
            for (int j = 0; j < map.Height; ++j)
            {
                map.SetTerrainAt(0, j, TerrainType.Wall);
                map.SetTerrainAt(map.Width - 1, j, TerrainType.Wall);
            }
        }

        protected void GenerateUpDownStairs(Map map, Stairs incommingStairs)
        {
            const int DistanceToKeepDownStairsFromUpStairs = 15;

            Stairs upStairs = map.MapObjects.Where(x => x.Type == MapObjectType.StairsUp).OfType<Stairs>().FirstOrDefault();
            if (upStairs == null)
            {
                upStairs = (Stairs)MapObjectFactory.Instance.CreateMapObject("StairsUp", GetClearPoint(map));
                map.AddMapItem(upStairs);
            }

            Point stairsDownPosition = GetClearPoint(map, upStairs.Position, DistanceToKeepDownStairsFromUpStairs, 5);
            Stairs downStairs = (Stairs)MapObjectFactory.Instance.CreateMapObject("StairsDown", stairsDownPosition);
            map.AddMapItem(downStairs);

            if (incommingStairs != null)
            {
                StairsMapping.Instance.SetMapping(incommingStairs.UniqueID, upStairs.Position);
                StairsMapping.Instance.SetMapping(upStairs.UniqueID, incommingStairs.Position);
            }
        }
    }
}
