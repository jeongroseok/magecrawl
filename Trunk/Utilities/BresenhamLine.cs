using System.Collections.Generic;
using libtcod;

namespace Magecrawl.Utilities
{
    public static class BresenhamLine
    {
        public static List<Point> GenerateLineList(Point startPoint, Point endPoint)
        {
            List<Point> returnList = new List<Point>();

            TCODLine.init(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

            int currentX = startPoint.X;
            int currentY = startPoint.Y;          
            while (!TCODLine.step(ref currentX, ref currentY))
            {
                returnList.Add(new Point(currentX, currentY));
            }

            return returnList;
        }
    }
}
