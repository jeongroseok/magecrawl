using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcodWrapper;

namespace Magecrawl.Utilities
{
    public static class BresenhamLine
    {
        public static List<Point> GenerateLineList(Point startPoint, Point endPoint)
        {
            List<Point> returnList = new List<Point>();

            TCODLineDrawing.InitLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

            int currentX = startPoint.X;
            int currentY = startPoint.Y;          
            while(!TCODLineDrawing.StepLine(ref currentX, ref currentY))
            {
                returnList.Add(new Point(currentX, currentY));
            }

            return returnList;
        }
    }
}
