using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class OperateKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        private Point SelectionPoint { get; set; }

        public OperateKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried()
        {
            List<EffectivePoint> listOfSelectablePoints = CalculateOperatePoints();
            if (listOfSelectablePoints.Count == 0)
            {
                m_gameInstance.TextBox.AddText("Nothing to operate on.");
                Escape();
                return;
            }

            SelectionPoint = SetOperateInitialSpot(m_engine);
            m_gameInstance.SendPaintersRequest("PlayerTargettingEnabled", listOfSelectablePoints);
            m_gameInstance.SendPaintersRequest("MapCursorEnabled", SelectionPoint);
            m_gameInstance.UpdatePainters();
        }

        private List<EffectivePoint> CalculateOperatePoints()
        {
            List<EffectivePoint> listOfSelectablePoints = new List<EffectivePoint>();

            foreach (IMapObject mapObj in m_engine.Map.MapObjects)
            {
                if (PointDirectionUtils.LatticeDistance(m_engine.Player.Position, mapObj.Position) == 1)
                {
                    listOfSelectablePoints.Add(new EffectivePoint(mapObj.Position, 1.0f));
                }
            }

            return listOfSelectablePoints;
        }

        private void Operate()
        {
            if (SelectionPoint != m_engine.Player.Position)
                m_engine.Operate(SelectionPoint);
            Escape();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest("MapCursorDisabled");
            m_gameInstance.SendPaintersRequest("PlayerTargettingDisabled");
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            Point pointWantToGoTo = PointDirectionUtils.ConvertDirectionToDestinationPoint(SelectionPoint, direction);
            Point resultPoint = TargetHandlerHelper.MoveSelectionToNewPoint(m_engine, pointWantToGoTo, direction, CalculateOperatePoints());
            if (resultPoint != Point.Invalid)
                SelectionPoint = resultPoint;
            m_gameInstance.SendPaintersRequest("MapCursorPositionChanged", SelectionPoint);
            m_gameInstance.UpdatePainters();
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

        // We're switching on operate, so find something nearby we can operate if possible
        private static Point SetOperateInitialSpot(IGameEngine engine)
        {
            foreach (IMapObject mapObject in engine.Map.MapObjects)
            {
                if (PointDirectionUtils.LatticeDistance(mapObject.Position, engine.Player.Position) == 1)
                    return mapObject.Position;
            }

            // If we can't find a better spot, use the player's position
            return engine.Player.Position;
        }
    }
}
