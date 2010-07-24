using System.Collections.Generic;
using libtcod;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal static class MonsterPlacer
    {
        private static TCODRandom m_random = new TCODRandom();

        // Priority is how dangerous this room is: 0 - unguarded, 10 - Must protect with everything
        internal static void PlaceMonster(Map map, Point upperLeft, Point lowerRight, List<Point> pointsNotToPlaceOn, int priority)
        {
            List<Point> pointsWithClearTerrain = MapGeneratorBase.GetClearPointListInRange(map, upperLeft, lowerRight);

            // We don't want monsters on top of map objects
            foreach (MapObject o in map.MapObjects)
                pointsWithClearTerrain.Remove(o.Position);

            // We need to remove seams since they might become walls later
            if (pointsNotToPlaceOn != null)
            {
                foreach (Point p in pointsNotToPlaceOn)
                    pointsWithClearTerrain.Remove(p);
            }

            pointsWithClearTerrain = pointsWithClearTerrain.Randomize();

            // TODO - Make this better.
            int numberOfMonstersToAdd = priority / 2;
            for (int i = 0; i < numberOfMonstersToAdd; ++i)
            {
                if (pointsWithClearTerrain.Count > 0)
                {
                    Point position = pointsWithClearTerrain[0];
                    pointsWithClearTerrain.RemoveAt(0);

                    map.AddMonster(CoreGameEngine.Instance.MonsterFactory.CreateRandomMonster(CoreGameEngine.Instance.CurrentLevel, position));
                }
            }
        }
    }
}
