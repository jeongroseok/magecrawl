using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.Keyboard
{
    internal sealed class AutoTraveler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        internal AutoTraveler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        private List<EffectivePoint> GeneratePointsOneCanMoveTo()
        {
            List<EffectivePoint> returnList = new List<EffectivePoint>();
            for (int i = 0; i < m_engine.Map.Width; ++i)
            {
                for (int j = 0; j < m_engine.Map.Height; ++j)
                {
                    Point p = new Point(i, j);
                    // We can move there is we've visited it is a floor and there is no solid object there.
                    if (m_engine.Map.GetTerrainAt(p) == TerrainType.Floor && m_engine.Map.IsVisitedAt(p) && m_engine.Map.MapObjects.Where(x => x.Position == p && x.IsSolid).Count() == 0)
                    {
                        returnList.Add(new EffectivePoint(p, 1));
                    }
                }
            }
            return returnList;
        }

        public void MoveToLocation(NamedKey movementKey)
        {
            List<EffectivePoint> targetPoints = GeneratePointsOneCanMoveTo();
            OnTargetSelection movementDelegate = new OnTargetSelection(OnMovementLocationSelected);
            m_gameInstance.SetHandlerName("Target", targetPoints, movementDelegate, movementKey, TargettingKeystrokeHandler.TargettingType.OpenFloor);
        }

        private bool OnMovementLocationSelected(Point selected)
        {
            // Don't show the overlap as we travel
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(false));
            m_gameInstance.SendPaintersRequest(new EnablePlayerTargeting(false));

            bool ableToMoveNextSquare = true;

            while (!m_engine.DangerInLOS() && ableToMoveNextSquare)
            {
                List<Point> pathToPoint = m_engine.PlayerPathToPoint(selected);
                if (pathToPoint == null || pathToPoint.Count == 0)
                    return false;

                Direction d = PointDirectionUtils.ConvertTwoPointsToDirection(m_engine.Player.Position, pathToPoint[0]);

                ableToMoveNextSquare = m_engine.MovePlayer(d);
                m_gameInstance.UpdatePainters();

                m_gameInstance.DrawFrame();
            }
            return false;
        }
    }
}
