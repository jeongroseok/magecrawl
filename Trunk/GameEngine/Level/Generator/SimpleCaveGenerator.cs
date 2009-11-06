using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal class SimpleCaveGenerator : MapGeneratorBase, IDisposable
    {
        internal SimpleCaveGenerator() : base()
        {
        }    

        internal Map GenerateMap(out Point playerPosition)
        {
            int width = m_random.GetRandomInt(40, 60); 
            int height = m_random.GetRandomInt(40, 60);
            Map map = new Map(width, height);

            // Fill non-sides with random bits
            for (int i = 1; i < width - 1; ++i)
            {
                for (int j = 1; j < height - 1; ++j)
                {
                    if (m_random.Chance(40))
                        map.GetInternalTile(i, j).Terrain = TerrainType.Wall;
                    else
                        map.GetInternalTile(i, j).Terrain = TerrainType.Floor;
                }
            }

            // Fill edges with walls
            for (int i = 0; i < width; ++i)
            {
                map.GetInternalTile(i, 0).Terrain = TerrainType.Wall;
                map.GetInternalTile(i, height - 1).Terrain = TerrainType.Wall;
            }
            for (int j = 0; j < height; ++j)
            {
                map.GetInternalTile(0, j).Terrain = TerrainType.Wall;
                map.GetInternalTile(width - 1, j).Terrain = TerrainType.Wall;
            }

            // For 4 iterators, apply 1stset 1 rules...
            // If we're near(1 tile) 5 or more walls
            // or close to (2 tiles) 2 or less walls
            // we're now a wall
            for (int z = 0; z < 4; ++z)
            {
                // Create a local copy to modify. We need both the old and new values for this calculation
                Map currentIterationMap = map.CopyMap();
                
                // Walk tiles, applying rule to local copy
                for (int i = 1; i < width - 1; ++i)
                {
                    for (int j = 1; j < height - 1; ++j)
                    {
                        int closeWalls = CountNumberOfSurroundingWallTilesOneStepAway(map, i, j);
                        int farWalls = CountNumberOfSurroundingWallTilesTwoStepAway(map, i, j);
                        bool conditionOne = closeWalls >= 5;
                        bool conditionTwo = farWalls <= 2;
                        if (conditionOne || conditionTwo)
                            currentIterationMap.GetInternalTile(i, j).Terrain = TerrainType.Wall;
                        else
                            currentIterationMap.GetInternalTile(i, j).Terrain = TerrainType.Floor;
                    }
                }

                // Push our changes out
                map = currentIterationMap;
            }
           
            // For 4 iterators, apply 2nd set of rules...
            // If we're near(1 tile) 5 or more walls we're now a wall
            for (int z = 0; z < 4; ++z)
            {
                // Create a local copy to modify. We need both the old and new values for this calculation
                Map currentIterationMap = map.CopyMap();

                // Walk tiles, applying rule to local copy
                for (int i = 1; i < width - 1; ++i)
                {
                    for (int j = 1; j < height - 1; ++j)
                    {
                        int closeWalls = CountNumberOfSurroundingWallTilesOneStepAway(map, i, j);

                        if (closeWalls >= 5)
                            currentIterationMap.GetInternalTile(i, j).Terrain = TerrainType.Wall;
                        else
                            currentIterationMap.GetInternalTile(i, j).Terrain = TerrainType.Floor;
                    }
                }

                // Push our changes out
                map = currentIterationMap;
            }

            FillAllSmallerUnconnectedRooms(map);

            playerPosition = GetClearPoint(map);

            GenerateMonstersAndChests(map, playerPosition);

            return map;
        }
    }
}
