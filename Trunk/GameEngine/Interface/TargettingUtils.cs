using System.Collections.Generic;
using Magecrawl.GameEngine.Magic;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interface
{
    internal class TargettingUtils : ITargettingUtils
    {
        private CoreGameEngine m_engine;

        public TargettingUtils(CoreGameEngine engine)
        {
            m_engine = engine;
        }

        public List<Point> PlayerPathToPoint(Point dest)
        {
            return m_engine.PathToPoint(m_engine.Player, dest, true, true, false);
        }

        public void FilterNotTargetableToPlayerPointsFromList(List<EffectivePoint> pointList, bool needsToBeVisible)
        {
            m_engine.FilterNotTargetablePointsFromList(pointList, m_engine.Player.Position, m_engine.Player.Vision, needsToBeVisible);
        }

        public void FilterNotVisibleToPlayerBothWaysFromList(List<EffectivePoint> pointList, bool savePlayerPositionFromList)
        {
            if (savePlayerPositionFromList)
                m_engine.FilterNotVisibleBothWaysFromList(m_engine.Player.Position, pointList, CoreGameEngine.Instance.Player.Position);
            else
                m_engine.FilterNotVisibleBothWaysFromList(m_engine.Player.Position, pointList, Point.Invalid);
        }

        public bool IsRangedPathBetweenPoints(Point x, Point y)
        {
            return m_engine.IsRangedPathBetweenPoints(x, y);
        }

        public List<Point> TargettedDrawablePoints(object targettingObject, Point target)
        {
            return m_engine.TargettedDrawablePoints(targettingObject, target);
        }

        public TargetingInfo GetTargettingTypeForInventoryItem(IItem item, string action)
        {
            if (action == "Drop")
                return null;

            Item coreItem = (Item)item;
            if (coreItem.ContainsAttribute("Invokable"))
                return SpellFactory.Instance.CreateSpell(coreItem.GetAttribute("InvokeSpellEffect")).Targeting;

            return null;
        }
    }
}
