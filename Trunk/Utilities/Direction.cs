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
        public static List<Direction> GenerateRandomDirectionList()
        {
            return GenerateDirectionList().Randomize();
        }

        public static List<Direction> GenerateDirectionList()
        {
            return new List<Direction>() { Direction.North, Direction.Northeast, Direction.East, Direction.Southeast, 
                Direction.South, Direction.Southwest, Direction.West, Direction.Northwest};
        }
    }
}
