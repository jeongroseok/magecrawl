using System;
using System.Collections.Generic;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.Keyboard
{
    // The code to get the ranged attack placement and movement of cursor is a bit scary. 
    // Moved it here from DefaultKeystrokeHandler for clarity.
    internal static class AttackKeystrokeHelper
    {
        // We're switching on a weapon, so target a random monster in range if there is one
        public static Point SetAttackInitialSpot(IGameEngine engine, IWeapon currentWeapon)
        {
            foreach (ICharacter m in engine.Map.Monsters)
            {
                if (currentWeapon.PositionInTargetablePoints(engine.Player.Position, m.Position))
                    return m.Position;
            }
            
            // If we can't find a better spot, use the player's position
            return engine.Player.Position;
        }

        /// <summary>
        /// This is called when moving the selection cursor. Since we don't allow invalid tiles to be selected, 
        /// we have to 'guess' when the player moves over gaps and edges. See MoveSelectionToNewPointSearchDirection for
        /// details on that.
        /// </summary>
        /// <param name="engine">GameEngine</param>
        /// <param name="pointWantToGoTo">The ideal point to move to if valid</param>
        /// <param name="direction">Direction of keypress</param>
        /// <returns>Point to move selectionto, Point.Invalid if can't move</returns>
        public static Point MoveSelectionToNewPoint(IGameEngine engine, Point pointWantToGoTo, Direction direction)
        {    
            // First try and see if we can just target that square
            if (engine.Player.CurrentWeapon.PositionInTargetablePoints(engine.Player.Position, pointWantToGoTo))
            {
                return pointWantToGoTo;
            }

            Point searchResult = MoveSelectionToNewPointSearchDirection(engine, pointWantToGoTo, direction, null);
            if (searchResult != Point.Invalid)
                return searchResult;

            // If not, we want to see if there's a square in that direction we can go, then try up/down a row
            List<Point> adjacentOffset = null;
            if (direction == Direction.North || direction == Direction.South)
                adjacentOffset = new List<Point>() { new Point(-1, 0), new Point(1, 0) };
            if (direction == Direction.East || direction == Direction.West)
                adjacentOffset = new List<Point>() { new Point(0, -1), new Point(0, 1) };
            searchResult = MoveSelectionToNewPointSearchDirection(engine, pointWantToGoTo, direction, adjacentOffset);
            
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
        private static Point MoveSelectionToNewPointSearchDirection(IGameEngine engine, Point pointWantToGoTo, Direction directionFromCenter, List<Point> offsets)
        {
            Point nextSelectionAttempt = pointWantToGoTo;
            const int SelectionSearchLength = 8;
            for (int i = 0; i < SelectionSearchLength; ++i)
            {
                if (i != 0)
                    nextSelectionAttempt = PointDirectionUtils.ConvertDirectionToDestinationPoint(nextSelectionAttempt, directionFromCenter);
                if (engine.Player.CurrentWeapon.PositionInTargetablePoints(engine.Player.Position, nextSelectionAttempt))
                {
                    return nextSelectionAttempt;
                }
                if (offsets != null)
                {
                    foreach (Point o in offsets)
                    {
                        if (engine.Player.CurrentWeapon.PositionInTargetablePoints(engine.Player.Position, nextSelectionAttempt + o))
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
