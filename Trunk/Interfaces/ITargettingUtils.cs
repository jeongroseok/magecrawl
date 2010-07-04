using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface ITargettingUtils
    {
        List<Point> PlayerPathToPoint(Point dest);

        bool IsRangedPathBetweenPoints(Point x, Point y);
        void FilterNotVisibleBothWaysFromList(List<EffectivePoint> pointList, bool savePlayerPositionFromList);

        void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, bool needsToBeVisible);

        // Takes either an IItem or ISpell
        List<Point> TargettedDrawablePoints(object targettingObject, Point target);
        
        TargetingInfo GetTargettingTypeForInventoryItem(IItem item, string action);
    }
}
