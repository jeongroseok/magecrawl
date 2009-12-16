using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl
{
    public enum Direction
    {
        None, North, Northeast, East, Southeast, South, Southwest, West, Northwest 
    }

    public static class DirectionUtils
    {
        public static List<Direction> GenerateDirectionList()
        {
            List<Direction> directionList = new List<Direction>();
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                if (d != Direction.None)
                    directionList.Add(d);
            }
            return directionList.Randomize();
        }
    }
}
