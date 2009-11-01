using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Utilities
{
    public static class PointDirectionUtils
    {
        public static Point ConvertDirectionToDestinationPoint(Point initial, Direction direction)
        {
            Point destPoint;
            switch (direction)
            {
                case Direction.North:
                    destPoint = new Point(initial.X, initial.Y - 1);
                    break;
                case Direction.South:
                    destPoint = new Point(initial.X, initial.Y + 1);
                    break;
                case Direction.West:
                    destPoint = new Point(initial.X - 1, initial.Y);
                    break;
                case Direction.East:
                    destPoint = new Point(initial.X + 1, initial.Y);
                    break;
                case Direction.Northeast:
                    destPoint = new Point(initial.X + 1, initial.Y - 1);
                    break;
                case Direction.Northwest:
                    destPoint = new Point(initial.X - 1, initial.Y - 1);
                    break;
                case Direction.Southeast:
                    destPoint = new Point(initial.X + 1, initial.Y + 1);
                    break;
                case Direction.Southwest:
                    destPoint = new Point(initial.X - 1, initial.Y + 1);
                    break;
                default:
                    throw new ArgumentException("ConvertDirectionToDestinationPoint - Invalid Direction");
            }
            return destPoint;
        }

        public static Direction ConvertTwoPointsToDirection(Point initial, Point end)
        {
            int x = end.X - initial.X;
            int y = end.Y - initial.Y;
            if (x > 1)
                x = 1;
            if (x < -1)
                x = -1;
            if (y > 1)
                y = 1;
            if (y < -1)
                y = -1;
            return ConvertPositionDeltaToDirection(x, y);
        }

        public static Direction ConvertPositionDeltaToDirection(int deltaX, int deltaY)
        {
            if (deltaX == 1)
            {
                if (deltaY == 1)
                    return Direction.Southeast;
                else if (deltaY == -1)
                    return Direction.Northeast;
                else
                    return Direction.East;
            }
            else if (deltaX == -1)
            {
                if (deltaY == 1)
                    return Direction.Southwest;
                else if (deltaY == -1)
                    return Direction.Northwest;
                else
                    return Direction.West;
            }
            else
            {
                if (deltaY == 1)
                    return Direction.South;
                else if (deltaY == -1)
                    return Direction.North;
                else
                    throw new System.ArgumentOutOfRangeException("ConvertPositionDeltaToDirection - No direction?");
            }
        }

        public static int LatticeDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
    }
}
