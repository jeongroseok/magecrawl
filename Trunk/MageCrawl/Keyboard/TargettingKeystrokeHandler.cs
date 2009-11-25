using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    // If true, don't reset the handler because we already set it to something new
    internal delegate bool OnTargetSelection(Point selection);

    internal class TargettingKeystrokeHandler : BaseKeystrokeHandler
    {
        internal enum TargettingType { None, Monster, Operatable };

        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private List<EffectivePoint> m_targetablePoints;
        private OnTargetSelection m_selectionDelegate;
        private NamedKey m_alternateSelectionKey;
        private bool m_doNotResetHandler;
        private TargettingType m_targettingType;

        private Point SelectionPoint { get; set; }

        public TargettingKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_targettingType = TargettingType.None;
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            // If we match the alternate key, call Select()
            if (m_alternateSelectionKey != NamedKey.Invalid && m_alternateSelectionKey == keystroke)
            {
                Select();
                return;
            }
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                try
                {
                    action.Invoke(this, null);
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            }
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            m_targetablePoints = (List<EffectivePoint>)objOne;
            m_selectionDelegate = (OnTargetSelection)objTwo;
            
            if (objThree != null)
                m_alternateSelectionKey = (NamedKey)objThree;
            else
                m_alternateSelectionKey = NamedKey.Invalid;

            m_targettingType = (TargettingType)objFour;

            SelectionPoint = SetTargettingInitialSpot(m_engine);

            m_gameInstance.SendPaintersRequest(new EnablePlayerTargeting(true, m_targetablePoints));
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(true, SelectionPoint));

            // If we have no targetable points, just exit now
            if (m_targetablePoints.Count == 0)
                Escape();

            m_gameInstance.UpdatePainters();
        }

        private void Select()
        {
            Attack();
        }

        private void Attack()
        {
            m_doNotResetHandler = m_selectionDelegate(SelectionPoint);
            Escape();
        }

        private void HandleDirection(Direction direction)
        {
            Point pointWantToGoTo = PointDirectionUtils.ConvertDirectionToDestinationPoint(SelectionPoint, direction);
            Point resultPoint = TargetHandlerHelper.MoveSelectionToNewPoint(m_engine, pointWantToGoTo, direction, m_targetablePoints);
            if (resultPoint != Point.Invalid)
                SelectionPoint = resultPoint;
            m_gameInstance.SendPaintersRequest(new ChangeCursorPosition(SelectionPoint));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(false));
            m_gameInstance.SendPaintersRequest(new EnablePlayerTargeting(false));
            m_selectionDelegate = null;
            m_gameInstance.UpdatePainters();

            if (!m_doNotResetHandler)
                m_gameInstance.ResetHandlerName();
            m_doNotResetHandler = false;
        }

        private void North()
        {
            HandleDirection(Direction.North);
        }

        private void South()
        {
            HandleDirection(Direction.South);
        }

        private void East()
        {
            HandleDirection(Direction.East);
        }

        private void West()
        {
            HandleDirection(Direction.West);
        }

        private void Northeast()
        {
            HandleDirection(Direction.Northeast);
        }

        private void Northwest()
        {
            HandleDirection(Direction.Northwest);
        }

        private void Southeast()
        {
            HandleDirection(Direction.Southeast);
        }

        private void Southwest()
        {
            HandleDirection(Direction.Southwest);
        }

        private Point SetTargettingInitialSpot(IGameEngine engine)
        {
            if (m_targettingType == TargettingType.Monster)
            {
                foreach (ICharacter m in engine.Map.Monsters)
                {
                    if (EffectivePoint.PositionInTargetablePoints(m.Position, m_targetablePoints))
                        return m.Position;
                }
            }
            else if (m_targettingType == TargettingType.Operatable)
            {
                foreach (IMapObject m in engine.Map.MapObjects.Where(x => x.CanOperate))
                {
                    if (EffectivePoint.PositionInTargetablePoints(m.Position, m_targetablePoints))
                        return m.Position;
                }
            }
            
            // If we can't find a better spot, use the player's position
            return engine.Player.Position;
        }
    }
}
