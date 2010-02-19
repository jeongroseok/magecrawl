using System.Collections.Generic;
using Magecrawl.Keyboard.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal abstract class InvokingKeystrokeHandler : BaseKeystrokeHandler
    {
        internal delegate void OnKeyboardInvoke(Point p);

        internal void HandleInvoke(NamedKey invokeKey, string targettingNeeded, int range, OnKeyboardInvoke onInvoke)
        {
            switch (targettingNeeded)
            {
                case "Single Range":
                {
                    List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(m_engine.Player.Position, range);
                    m_engine.FilterNotTargetablePointsFromList(targetablePoints, true);
                    m_engine.FilterNotVisibleBothWaysFromList(targetablePoints);

                    OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
                    {
                        if (s != m_engine.Player.Position)
                            onInvoke(s);
                        return false;
                    });
                    m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetablePoints, selectionDelegate, invokeKey, TargettingKeystrokeHandler.TargettingType.Monster));
                    break;
                }
                case "Cone":
                {
                    Point playerPosition = m_engine.Player.Position;
                    List<EffectivePoint> targetablePoints = new List<EffectivePoint>();
                    targetablePoints.Add(new EffectivePoint(playerPosition + new Point(0, 1), 1.0f));
                    targetablePoints.Add(new EffectivePoint(playerPosition + new Point(0, -1), 1.0f));
                    targetablePoints.Add(new EffectivePoint(playerPosition + new Point(1, 0), 1.0f));
                    targetablePoints.Add(new EffectivePoint(playerPosition + new Point(-1, 0), 1.0f));
                    m_engine.FilterNotTargetablePointsFromList(targetablePoints, true);
                    m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetablePoints, new OnTargetSelection(x => { onInvoke(x); return false; }),
                        NamedKey.Invalid, TargettingKeystrokeHandler.TargettingType.Monster));
                    break;
                }
                case "Self":
                {
                    onInvoke(m_engine.Player.Position);
                    m_gameInstance.ResetHandlerName();
                    m_gameInstance.UpdatePainters();
                    break;
                }
                default:
                    throw new System.ArgumentException("Don't know how to invoke with target type: " + targettingNeeded);
            }
        }
    }
}
