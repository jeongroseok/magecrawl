using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Keyboard.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal abstract class InvokingKeystrokeHandler : BaseKeystrokeHandler
    {
        internal delegate void OnKeyboardInvoke(Point p);

        internal void HandleInvoke(object invokingObject, TargetingInfo targetInfo, OnKeyboardInvoke onInvoke, NamedKey invokeKey)
        {
            if (targetInfo == null)
            {
                onInvoke(m_engine.Player.Position);
                m_gameInstance.ResetHandlerName();
                m_gameInstance.UpdatePainters();
                return;
            }
            else
            {
                switch (targetInfo.Type)
                {
                    case TargetingInfo.TargettingType.Stream:
                    {
                        List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListOneStepAllDirections(m_engine.Player.Position);
                        HandleRangedSinglePointInvoke(invokingObject, targetablePoints, onInvoke, invokeKey);
                        return;
                    }
                    case TargetingInfo.TargettingType.RangedSingle:
                    case TargetingInfo.TargettingType.RangedBlast:
                    case TargetingInfo.TargettingType.RangedExplodingPoint:
                    {
                        List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(m_engine.Player.Position, targetInfo.Range);
                        HandleRangedSinglePointInvoke(invokingObject, targetablePoints, onInvoke, invokeKey);
                        return;
                    }
                    case TargetingInfo.TargettingType.Cone:
                    {
                        Point playerPosition = m_engine.Player.Position;
                        List<EffectivePoint> targetablePoints = GetConeTargetablePoints(playerPosition);
                        OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
                        {
                            if (s != m_engine.Player.Position)
                                onInvoke(s);
                            return false;
                        });
                        m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetablePoints, selectionDelegate,
                            NamedKey.Invalid, TargettingKeystrokeHandler.TargettingType.Monster,
                            p => m_engine.Targetting.TargettedDrawablePoints(invokingObject, p)));
                        return;
                    }
                    case TargetingInfo.TargettingType.Self:
                    {
                        onInvoke(m_engine.Player.Position);
                        m_gameInstance.ResetHandlerName();
                        m_gameInstance.UpdatePainters();
                        return;
                    }
                    default:
                        throw new System.InvalidOperationException("InvokingKeystrokeHandler - HandleInvoke, don't know how to handle: " + targetInfo.Type.ToString());
                }
            }
        }

        private void HandleRangedSinglePointInvoke(object invokingObject, List<EffectivePoint> targetablePoints, OnKeyboardInvoke onInvoke, NamedKey invokeKey)
        {
            m_engine.Targetting.FilterNotTargetableToPlayerPointsFromList(targetablePoints, true);
            m_engine.Targetting.FilterNotVisibleToPlayerBothWaysFromList(targetablePoints, true);

            OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
            {
                if (s != m_engine.Player.Position)
                    onInvoke(s);
                return false;
            });
            m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetablePoints, selectionDelegate, invokeKey,
                TargettingKeystrokeHandler.TargettingType.Monster, p => m_engine.Targetting.TargettedDrawablePoints(invokingObject, p)));
        }

        private List<EffectivePoint> GetConeTargetablePoints(Point playerPosition)
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();
            targetablePoints.Add(new EffectivePoint(playerPosition + new Point(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(playerPosition + new Point(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(playerPosition + new Point(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(playerPosition + new Point(-1, 0), 1.0f));
            m_engine.Targetting.FilterNotTargetableToPlayerPointsFromList(targetablePoints, true);
            return targetablePoints;
        }
    }
}
