using System;
using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public static class PointListUtils
    {
        public static List<Point> PointListFromCone(Point center, Direction d, int coneLength)
        {
            if (d == Direction.Northeast || d == Direction.Northwest || d == Direction.Southeast || d == Direction.Southwest)
                throw new NotImplementedException("Codes on diagonals not implemented yet");

            if (d == Direction.None)
                return null;

            List<Point> affectedPoints = new List<Point>();

            Point firstPointInDirection = PointDirectionUtils.ConvertDirectionToDestinationPoint(center, d);
            if (center == firstPointInDirection)
                return affectedPoints;

            int deltaX = firstPointInDirection.X - center.X;
            int deltaY = firstPointInDirection.Y - center.Y;
            Point coneCenterForDistance = firstPointInDirection;
            for (int i = 0; i < coneLength; ++i)
            {
                affectedPoints.Add(coneCenterForDistance);
                for (int z = 0; z < i + 1; ++z)
                {
                    if (deltaX != 0)
                    {
                        affectedPoints.Add(new Point(coneCenterForDistance.X, coneCenterForDistance.Y - (z + 1)));
                        affectedPoints.Add(new Point(coneCenterForDistance.X, coneCenterForDistance.Y + (z + 1)));
                    }
                    else
                    {
                        affectedPoints.Add(new Point(coneCenterForDistance.X - (z + 1), coneCenterForDistance.Y));
                        affectedPoints.Add(new Point(coneCenterForDistance.X + (z + 1), coneCenterForDistance.Y));
                    }
                }
                coneCenterForDistance = new Point(coneCenterForDistance.X + deltaX, coneCenterForDistance.Y + deltaY);
            }
            return affectedPoints;
        }

        public static List<EffectivePoint> EffectivePointListFromBurstPosition(Point center, int burstDistance)
        {
            List<EffectivePoint> returnValue = new List<EffectivePoint>();
            for (int i = -burstDistance; i <= burstDistance; ++i)
            {
                for (int j = -burstDistance; j <= burstDistance; ++j)
                {
                    int distanceFromCenter = System.Math.Abs(i) + Math.Abs(j);
                    if (distanceFromCenter <= burstDistance)
                    {
                        returnValue.Add(new EffectivePoint(new Point(center.X + i, center.Y + j), 1.0f));
                    }
                }
            }
            return returnValue;
        }

        public static List<Point> PointListFromBurstPosition(Point center, int burstDistance)
        {
            List<Point> returnValue = new List<Point>();
            for (int i = -burstDistance; i <= burstDistance; ++i)
            {
                for (int j = -burstDistance; j <= burstDistance; ++j)
                {
                    int distanceFromCenter = System.Math.Abs(i) + Math.Abs(j);
                    if (distanceFromCenter <= burstDistance)
                    {
                        returnValue.Add(new Point(center.X + i, center.Y + j));
                    }
                }
            }
            return returnValue;
        }
    }
}
