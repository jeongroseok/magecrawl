using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class AttackKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        private Point SelectionPoint { get; set; }

        public AttackKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried()
        {
            SelectionPoint = SetAttackInitialSpot(m_engine, m_engine.Player.CurrentWeapon);
            List<EffectivePoint> listOfSelectablePoints = m_engine.Player.CurrentWeapon.CalculateTargetablePoints();
            m_gameInstance.SendPaintersRequest("PlayerTargettingEnabled", listOfSelectablePoints);
            m_gameInstance.SendPaintersRequest("MapCursorEnabled", SelectionPoint);
            m_gameInstance.UpdatePainters();
        }

        private void Attack()
        {
            if (SelectionPoint != m_engine.Player.Position)
                m_engine.PlayerAttack(SelectionPoint);
            Escape();
        }

        private void HandleDirection(Direction direction)
        {
            Point pointWantToGoTo = PointDirectionUtils.ConvertDirectionToDestinationPoint(SelectionPoint, direction);
            Point resultPoint = TargetHandlerHelper.MoveSelectionToNewPoint(m_engine, pointWantToGoTo, direction, m_engine.Player.CurrentWeapon.CalculateTargetablePoints());
            if (resultPoint != Point.Invalid)
                SelectionPoint = resultPoint;
            m_gameInstance.SendPaintersRequest("MapCursorPositionChanged", SelectionPoint);
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest("MapCursorDisabled");
            m_gameInstance.SendPaintersRequest("PlayerTargettingDisabled");
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

        // We're switching on a weapon, so target a random monster in range if there is one
        private static Point SetAttackInitialSpot(IGameEngine engine, IWeapon currentWeapon)
        {
            List<EffectivePoint> targetablePoints = engine.Player.CurrentWeapon.CalculateTargetablePoints();

            foreach (ICharacter m in engine.Map.Monsters)
            {
                if (EffectivePoint.PositionInTargetablePoints(m.Position, targetablePoints))
                    return m.Position;
            }
            
            // If we can't find a better spot, use the player's position
            return engine.Player.Position;
        }

    }
}
