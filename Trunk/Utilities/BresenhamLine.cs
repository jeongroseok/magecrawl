using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.Utilities
{
    // This code was taken from bresenham_c.c from libtcod
    // This code is except from many of the normal coding conversions
    // as it is a port of C code
    public class BresenhamLine
    {
        private int stepx;
        private int stepy;
        private int e;
        private int deltax;
        private int deltay;
        private int origx;
        private int origy;
        private int destx;
        private int desty;

        public BresenhamLine(Point from, Point to)
        {
            origx = from.X;
            origy = from.Y;
            destx = to.X;
            desty = to.Y;
            deltax = to.X - from.X;
            deltay = to.Y - from.Y;
            if (deltax > 0)
            {
                stepx = 1;
            }
            else if (deltax < 0)
            {
                stepx = -1;
            }
            else stepx = 0;
            if (deltay > 0)
            {
                stepy = 1;
            }
            else if (deltay < 0)
            {
                stepy = -1;
            }
            else stepy = 0;
            if (stepx * deltax > stepy * deltay)
            {
                e = stepx * deltax;
                deltax *= 2;
                deltay *= 2;
            }
            else
            {
                e = stepy * deltay;
                deltax *= 2;
                deltay *= 2;
            }
        }

        public Point Step()
        {
            if (stepx * deltax > stepy * deltay)
            {
                if (origx == destx) 
                    return Point.Invalid;
                origx += stepx;
                e -= stepy * deltay;
                if (e < 0)
                {
                    origy += stepy;
                    e += stepx * deltax;
                }
            }
            else
            {
                if (origy == desty)
                    return Point.Invalid;
                origy += stepy;
                e -= stepx * deltax;
                if (e < 0)
                {
                    origx += stepx;
                    e += stepy * deltay;
                }
            }
            return new Point(origx, origy);
        }
    }
}
