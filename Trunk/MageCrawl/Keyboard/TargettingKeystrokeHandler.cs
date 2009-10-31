using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal delegate void OnTargetSelection(Point selection);

    internal class TargettingKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private List<EffectivePoint> m_targetablePoints;
        private bool m_allowEscapeFromMode;
        private OnTargetSelection m_selectionDelegate;
        private NamedKey m_alternateSelectionKey;

        private Point SelectionPoint { get; set; }

        public TargettingKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_allowEscapeFromMode = true;
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            // If we match the alternate key, call Select()
            if (m_alternateSelectionKey.Code == keystroke.Code)
            {
                if (m_alternateSelectionKey.Code != libtcodWrapper.KeyCode.TCODK_CHAR || 
                    m_alternateSelectionKey.Character == keystroke.Character)
                {
                    Select();
                    return;
                }
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
            m_alternateSelectionKey = (NamedKey)objThree;
            if (objFour != null)
                SelectionPoint = (Point)objFour;
            else
                SelectionPoint = SetTargettingInitialSpot(m_engine);
            m_gameInstance.SendPaintersRequest("PlayerTargettingEnabled", m_targetablePoints);
            m_gameInstance.SendPaintersRequest("MapCursorEnabled", SelectionPoint);

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
            m_selectionDelegate(SelectionPoint);
            Escape();
        }

        private void HandleDirection(Direction direction)
        {
            Point pointWantToGoTo = PointDirectionUtils.ConvertDirectionToDestinationPoint(SelectionPoint, direction);
            Point resultPoint = TargetHandlerHelper.MoveSelectionToNewPoint(m_engine, pointWantToGoTo, direction, m_targetablePoints);
            if (resultPoint != Point.Invalid)
                SelectionPoint = resultPoint;
            m_gameInstance.SendPaintersRequest("MapCursorPositionChanged", SelectionPoint);
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            if (m_allowEscapeFromMode)
            {
                ExitMode();
            }
        }

        private void ExitMode()
        {
            m_gameInstance.SendPaintersRequest("MapCursorDisabled");
            m_gameInstance.SendPaintersRequest("PlayerTargettingDisabled");
            m_selectionDelegate = null;
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
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

        // If not overridden, target a monster in range if there is one.
        private Point SetTargettingInitialSpot(IGameEngine engine)
        {
            foreach (ICharacter m in engine.Map.Monsters)
            {
                if (EffectivePoint.PositionInTargetablePoints(m.Position, m_targetablePoints))
                    return m.Position;
            }
            
            // If we can't find a better spot, use the player's position
            return engine.Player.Position;
        }
    }
}
