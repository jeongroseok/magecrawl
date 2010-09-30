using System.Collections.Generic;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    internal static class TargetHandlerHelper
    {
        /// <summary>
        /// This is called when moving the selection cursor. Since we don't allow invalid tiles to be selected, 
        /// we have to 'guess' when the player moves over gaps and edges. See MoveSelectionToNewPointSearchDirection for
        /// details on that.
        /// </summary>
        /// <param name="engine">GameEngine</param>
        /// <param name="pointWantToGoTo">The ideal point to move to if valid</param>
        /// <param name="direction">Direction of keypress</param>
        /// <returns>Point to move selectionto, Point.Invalid if can't move</returns>
        internal static Point MoveSelectionToNewPoint(IGameEngine engine, Point pointWantToGoTo, Direction direction, List<EffectivePoint> targetablePoints)
        {
            // First try and see if we can just target that square
            if (targetablePoints == null || EffectivePoint.PositionInTargetablePoints(pointWantToGoTo, targetablePoints))
            {
                return pointWantToGoTo;
            }

            Point searchResult = MoveSelectionToNewPointSearchDirection(engine, pointWantToGoTo, direction, null, targetablePoints);
            if (searchResult != Point.Invalid)
                return searchResult;

            // If not, we want to see if there's a square in that direction we can go, then try up/down a row
            List<Point> adjacentOffset = null;
            if (direction == Direction.North || direction == Direction.South)
                adjacentOffset = new List<Point>() { new Point(-1, 0), new Point(1, 0) };
            if (direction == Direction.East || direction == Direction.West)
                adjacentOffset = new List<Point>() { new Point(0, -1), new Point(0, 1) };
            searchResult = MoveSelectionToNewPointSearchDirection(engine, pointWantToGoTo, direction, adjacentOffset, targetablePoints);

            return searchResult;
        }

        /// <summary>
        /// So the idea here is that we can't move to our desired spot, because it's not valid. 
        /// To try to find where the player wanted to go, we lineraly search SelectionSearchLength
        /// points in a direction looking for a good position. If nothing is found, MoveSelectionToNewPoint
        /// calls this again with an offset that has us look one tile in the perpendicular direction
        /// for a matching tile. This is so something like this
        ///   . 
        /// . @ .
        ///   . 
        /// can allow one from moving from the south to the east point.
        /// </summary>
        /// <param name="engine">Game Engine</param>
        /// <param name="pointWantToGoTo">Where was the original ideal point</param>
        /// <param name="directionFromCenter">What direction was this from the center</param>
        /// <param name="offsets">Which ways to shift if we're trying for nearby matches</param>
        /// <returns></returns>
        private static Point MoveSelectionToNewPointSearchDirection(IGameEngine engine, Point pointWantToGoTo, Direction directionFromCenter, List<Point> offsets, List<EffectivePoint> targetablePoints)
        {
            Point nextSelectionAttempt = pointWantToGoTo;
            const int SelectionSearchLength = 20;
            for (int i = 0; i < SelectionSearchLength; ++i)
            {
                if (i != 0)
                    nextSelectionAttempt = PointDirectionUtils.ConvertDirectionToDestinationPoint(nextSelectionAttempt, directionFromCenter);
                if (targetablePoints == null || EffectivePoint.PositionInTargetablePoints(nextSelectionAttempt, targetablePoints))
                {
                    return nextSelectionAttempt;
                }
                if (offsets != null)
                {
                    foreach (Point o in offsets)
                    {
                        if (targetablePoints == null || EffectivePoint.PositionInTargetablePoints(nextSelectionAttempt + o, targetablePoints))
                        {
                            return nextSelectionAttempt + o;
                        }
                    }
                }
            }
            return Point.Invalid;
        }
    }
}
