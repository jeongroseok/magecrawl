using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal sealed class SimpleCaveGenerator : MapGeneratorBase
    {
        internal SimpleCaveGenerator(TCODRandom random) : base(random)
        {
        }    

        internal override Map GenerateMap(Point stairsUpPosition, out Point stairsDownPosition)
        {
            Map map;
            do
            {
                int width = m_random.GetRandomInt(40, Math.Max(60, stairsUpPosition.X + 10));
                int height = m_random.GetRandomInt(40, Math.Max(60, stairsUpPosition.Y + 10));
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
            }
            while (!HasValidStairPositioning(stairsUpPosition, map));

            GenerateUpDownStairs(map, stairsUpPosition, out stairsDownPosition);

            GenerateMonstersAndChests(map, stairsUpPosition);

            return map;
        }
    }
}
