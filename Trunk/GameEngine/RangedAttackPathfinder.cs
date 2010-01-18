using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Level;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal static class RangedAttackPathfinder
    {
        internal static List<Point> RangedListOfPoints(Map map, Point caster, Point target, bool continuePastTarget)
        {
            List<Point> returnList = GenerateListOfPointsSinglePass(map, caster, target);

            // The calcualted list might have caster as element 0
            if (returnList[0] == caster)
                returnList.RemoveAt(0);

            if (!returnList.Contains(target))
                return null;

            if (continuePastTarget)
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
                    startingPoint = endingPoint;
                    endingPoint = endingPoint + delta;
                }
            }

            return returnList;
        }

        private static List<Point> GenerateListOfPointsSinglePass(Map map, Point caster, Point target)
        {
            List<Point> returnList = new List<Point>();

            foreach (Point p in BresenhamLine.RenderLine(caster, target))
            {
                if (!ValidPoint(map, p))
                    break;
                returnList.Add(p);
            }

            // the Bresenham algorithsm doesn't add the end point.
            if (!returnList.Contains(target) && ValidPoint(map, target))
                returnList.Add(target);
            return returnList;
        }

        private static bool ValidPoint(Map map, Point p)
        {
            return map.IsPointOnMap(p) && !(map.GetTerrainAt(p) == TerrainType.Wall || map.MapObjects.Where(mapObj => mapObj.IsSolid && mapObj.Position == p).Count() > 0);
        }
    }
}
