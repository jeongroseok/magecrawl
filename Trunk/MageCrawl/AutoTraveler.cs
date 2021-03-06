﻿using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Interfaces;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Requests;
using Magecrawl.Utilities;

namespace Magecrawl
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
            m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetPoints, OnMovementLocationSelected, movementKey, TargettingKeystrokeHandler.TargettingType.OpenFloor));
        }

        private bool OnMovementLocationSelected(Point selected)
        {
            // Don't show the overlap as we travel
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(false));
            m_gameInstance.SendPaintersRequest(new EnablePlayerTargeting(false));

            bool ableToMoveNextSquare = true;

            while (!m_engine.GameState.DangerInLOS() && ableToMoveNextSquare)
            {
                // If user hits a key while traveling, stop
                if (TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed).Pressed)
                    break;

                List<Point> pathToPoint = m_engine.Targetting.PlayerPathToPoint(selected);
                if (pathToPoint == null || pathToPoint.Count == 0)
                    return false;

                Direction d = PointDirectionUtils.ConvertTwoPointsToDirection(m_engine.Player.Position, pathToPoint[0]);

                ableToMoveNextSquare = m_engine.Actions.Move(d);
                m_gameInstance.UpdatePainters();

                m_gameInstance.DrawFrame();
            }
            return false;
        }

        public void RunInDirection(Direction direction)
        {
            // Don't show the overlap as we travel
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(false));
            m_gameInstance.SendPaintersRequest(new EnablePlayerTargeting(false));

            bool ableToMoveNextSquare = true;

            while (!m_engine.GameState.DangerInLOS() && ableToMoveNextSquare)
            {
                // If user hits a key while traveling, stop
                if (TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed).Pressed)
                    break;

                ableToMoveNextSquare = m_engine.Actions.Move(direction);
                m_gameInstance.UpdatePainters();

                m_gameInstance.DrawFrame();
            }
        }
    }
}
