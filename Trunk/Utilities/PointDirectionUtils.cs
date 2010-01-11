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

        public static Direction GetDirectionOpposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                case Direction.East:
                    return Direction.West;
                case Direction.Northeast:
                    return Direction.Southwest;
                case Direction.Northwest:
                    return Direction.Northeast;
                case Direction.Southeast:
                    return Direction.Northwest;
                case Direction.Southwest:
                    return Direction.Northeast;
                default:
                    throw new ArgumentException("ConvertDirectionToDestinationPoint - Invalid Direction");
            }
        }

        public static List<Direction> GetDirectionsOpposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new List<Direction>() { Direction.Southwest, Direction.South, Direction.Southeast }.Randomize();
                case Direction.South:
                    return new List<Direction>() { Direction.Northwest, Direction.North, Direction.Northeast }.Randomize();
                case Direction.West:
                    return new List<Direction>() { Direction.Northeast, Direction.East, Direction.Southeast }.Randomize();
                case Direction.East:
                    return new List<Direction>() { Direction.Northwest, Direction.West, Direction.Southwest }.Randomize();
                case Direction.Northeast:
                    return new List<Direction>() { Direction.West, Direction.Southwest, Direction.South}.Randomize();
                case Direction.Northwest:
                    return new List<Direction>() { Direction.East, Direction.Southeast, Direction.South }.Randomize();
                case Direction.Southeast:
                    return new List<Direction>() { Direction.Northwest, Direction.North, Direction.West}.Randomize();
                case Direction.Southwest:
                    return new List<Direction>() { Direction.Northeast, Direction.North, Direction.East}.Randomize();
                default:
                    throw new ArgumentException("ConvertDirectionToDestinationPoint - Invalid Direction");
            }
        }

        public static int LatticeDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }

        public static double NormalDistance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }
    }
}
