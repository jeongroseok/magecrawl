using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Level;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal static class RangedAttackPathfinder
    {
        internal static List<Point> RangedListOfPoints(Map map, Point caster, Point target, bool continuePastTarget, bool bounceOffWalls)
        {
            if (caster == target)
                return null;

            List<Point> returnList = GenerateListOfPointsSinglePass(map, caster, target);

            // The calcualted list might have caster as element 0
            if (returnList.Count > 0 && returnList[0] == caster)
                returnList.RemoveAt(0);

            if (!returnList.Contains(target))
                return null;

            if (continuePastTarget)
            {
                {
                    Point delta = target - caster;
                    Point startingPoint = target;
                    Point endingPoint = target + delta;
                    while (true)
                    {
                        List<Point> listExtension = GenerateListOfPointsSinglePass(map, startingPoint, endingPoint);
                        if (listExtension.Count == 0)
                            break;
                        returnList.AddRange(listExtension);
                        
                        // If our extension didn't reach the ending point, we must have hit a wall.
                        if (!listExtension.Contains(endingPoint))
                            break;

                        startingPoint = endingPoint;
                        endingPoint = endingPoint + delta;
                    }
                }

                // At this point, we know that the target was passed (we didn't return null)
                // and that we won't still till we hit a wall. To reflect, we can just bounce the last few cells
                if (bounceOffWalls)
                {
                    const int BounceLength = 3;

                    Point lastPointInList = returnList.Last();

                    // "Bounce" towards caster, but we have to be a square past past us
                    Direction directionOfBounce = PointDirectionUtils.ConvertTwoPointsToDirection(lastPointInList, caster);
                    Point spotToAimBounce = caster;
                    for (int i = 0; i < BounceLength; i++)
                    {
                        Point newSpot = PointDirectionUtils.ConvertDirectionToDestinationPoint(spotToAimBounce, directionOfBounce);
                        if (ValidPoint(map, newSpot))
                            spotToAimBounce = newSpot;
                        else
                            break;
                    }

                    List<Point> bounceList = GenerateListOfPointsSinglePass(map, lastPointInList, spotToAimBounce);
                    bounceList.Insert(0, lastPointInList);  // The starting point gets hit twice
                    for (int i = 0; i < BounceLength && bounceList.Count > i; ++i)
                        returnList.Add(bounceList[i]);
                }
            }

            return returnList;
        }

        private static List<Point> GenerateListOfPointsSinglePass(Map map, Point caster, Point target)
        {
            List<Point> returnList = new List<Point>();

            foreach (Point p in BresenhamLine.GenerateLineList(caster, target))
            {
                if (!ValidPoint(map, p))
                    break;
                returnList.Add(p);
            }

            return returnList;
        }

        private static bool ValidPoint(Map map, Point p)
        {
            return map.IsPointOnMap(p) && !(map.GetTerrainAt(p) == TerrainType.Wall || map.MapObjects.Where(mapObj => mapObj.IsSolid && mapObj.Position == p).Count() > 0);
        }
    }
}
