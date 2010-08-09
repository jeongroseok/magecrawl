using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator.Cave
{
    internal sealed class SimpleCaveGenerator : MapGeneratorBase
    {
        internal SimpleCaveGenerator(TCODRandom random) : base(random)
        {
        }    

        internal override Map GenerateMap(Stairs incommingStairs, int level)
        {
            Map map;

            int width = m_random.getInt(40, 60);
            int height = m_random.getInt(40, 60);
            map = new Map(width, height);
            Map duplicateMap = new Map(width, height);

            // Fill non-sides with random bits
            for (int i = 1; i < width - 1; ++i)
            {
                for (int j = 1; j < height - 1; ++j)
                {
                    Point p = new Point(i, j);
                    if (m_random.Chance(40))
                        map.SetTerrainAt(p, TerrainType.Wall);
                    else
                        map.SetTerrainAt(p, TerrainType.Floor);
                }
            }

            FillEdgesWithWalls(map);

            // For 4 iterators, apply 1stset 1 rules...
            // If we're near(1 tile) 5 or more walls
            // or close to (2 tiles) 2 or less walls
            // we're now a wall
            for (int z = 0; z < 4; ++z)
            {
                // Create a local copy to modify. We need both the old and new values for this calculation
                duplicateMap.CopyMap(map);

                // Walk tiles, applying rule to local copy
                for (int i = 1; i < width - 1; ++i)
                {
                    for (int j = 1; j < height - 1; ++j)
                    {
                        Point p = new Point(i, j);
                        int closeWalls = CountNumberOfSurroundingWallTilesOneStepAway(map, i, j);
                        int farWalls = CountNumberOfSurroundingWallTilesTwoStepAway(map, i, j);
                        bool conditionOne = closeWalls >= 5;
                        bool conditionTwo = farWalls <= 2;
                        if (conditionOne || conditionTwo)
                            duplicateMap.SetTerrainAt(p, TerrainType.Wall);
                        else
                            duplicateMap.SetTerrainAt(p, TerrainType.Floor);
                    }
                }

                // Push our changes out
                map.CopyMap(duplicateMap);
            }

            // For 4 iterators, apply 2nd set of rules...
            // If we're near(1 tile) 5 or more walls we're now a wall
            for (int z = 0; z < 4; ++z)
            {
                // Create a local copy to modify. We need both the old and new values for this calculation
                duplicateMap.CopyMap(map);

                // Walk tiles, applying rule to local copy
                for (int i = 1; i < width - 1; ++i)
                {
                    for (int j = 1; j < height - 1; ++j)
                    {
                        int closeWalls = CountNumberOfSurroundingWallTilesOneStepAway(map, i, j);
                        Point p = new Point(i, j);

                        if (closeWalls >= 5)
                            duplicateMap.SetTerrainAt(p, TerrainType.Wall);
                        else
                            duplicateMap.SetTerrainAt(p, TerrainType.Floor);
                    }
                }

                // Push our changes out
                map.CopyMap(duplicateMap);
            }

            FillAllSmallerUnconnectedRooms(map);

            GenerateUpDownStairs(map, incommingStairs);

            Point stairsUpPosition = map.MapObjects.Where(x => x.Type == MapObjectType.StairsUp).First().Position;

            GenerateMonstersAndChests(map, stairsUpPosition, level);

            return map;
        }

        private const int SegmentSize = 10;
        private List<Point> GenerateSegmentList(Map map, Point pointToAvoid)
        {
            int numberOfXSegments = map.Width / SegmentSize;
            int numberOfYSegments = map.Height / SegmentSize;

            int avoidSectionX = pointToAvoid.X / SegmentSize;
            int avoidSectionY = pointToAvoid.Y / SegmentSize;

            List<Point> returnList = new List<Point>();
            for (int i = 0; i < numberOfXSegments; ++i)
            {
                for (int j = 0; j < numberOfYSegments; ++j)
                {
                    if (avoidSectionX == i && avoidSectionY == j)
                        continue;
                    returnList.Add(new Point(i, j));
                }
            }
            return returnList.Randomize();
        }

        private void GenerateMonstersAndChests(Map map, Point pointToAvoid, int level)
        {
            int treasureToGenerate = m_random.getInt(2, 4);
            int treasuresGenerated = 0;

            Point segmentSizedPoint = new Point(SegmentSize, SegmentSize);

            foreach (Point segment in GenerateSegmentList(map, pointToAvoid))
            {
                Point upperLeft = new Point(SegmentSize * segment.X, SegmentSize * segment.Y);
                Point lowerRight = upperLeft + segmentSizedPoint;

                List<Point> clearSegments = GetClearPointListInRange(map, upperLeft, lowerRight).Randomize();

                if (treasuresGenerated < treasureToGenerate && m_random.Chance(40))
                {
                    Point position = PopOffClearSegementList(clearSegments);
                    if (position != Point.Invalid)
                    {
                        MapObject treasure = CoreGameEngine.Instance.MapObjectFactory.CreateMapObject("TreasureChest", position);
                        map.AddMapItem(treasure);
                        treasuresGenerated++;
                    }
                }

                MonsterPlacer.PlaceMonster(map, upperLeft, lowerRight, null, m_random.getInt(2, 4), level);
            }
        }

        private static Point PopOffClearSegementList(List<Point> clearSegments)
        {
            if (clearSegments.Count > 0)
            {
                Point position = clearSegments[0];
                clearSegments.RemoveAt(0);
                return position;
            }
            else
                return Point.Invalid;
        }
    }
}
