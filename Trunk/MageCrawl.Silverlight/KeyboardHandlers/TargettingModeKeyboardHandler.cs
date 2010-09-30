using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

using MageCrawlPoint = Magecrawl.Utilities.Point;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public delegate void OnTargetSelect(GameWindow window, IGameEngine engine, MageCrawlPoint point);

    public class TargettingModeKeyboardHandler
    {
        public enum TargettingType
        {
            None, Monster, Operatable, OpenFloor
        }

        public TargettingModeKeyboardHandler(TargettingType targettingType, IGameEngine engine, Map map, List<EffectivePoint> targetablePoints)
        {
            m_targetablePoints = targetablePoints;

            m_targettingType = targettingType;
            
            map.InTargettingMode = true;
            map.TargetPoint = SetTargettingInitialSpot(engine);
            if (m_targetablePoints != null)
                map.TargetablePoints = m_targetablePoints;

            m_lastTargetted = null;
        }

        public TargettingModeKeyboardHandler(TargettingType targettingType, IGameEngine engine, Map map, OnTargetSelect selectionAction, List<EffectivePoint> targetablePoints)
            : this(targettingType, engine, map, targetablePoints)
        {
            m_action = selectionAction;
        }

        private TargettingType m_targettingType;
        private OnTargetSelect m_action;
        private List<EffectivePoint> m_targetablePoints;
        private ICharacter m_lastTargetted;

        public void OnKeyboardDown(MagecrawlKey key, Map map, GameWindow window, IGameEngine engine)
        {
            switch (key)
            {
                case MagecrawlKey.Enter:
                {
                    ICharacter possiblyTargettedMonster = engine.Map.Monsters.Where(x => x.Position == map.TargetPoint).FirstOrDefault();

                    // Rememeber last targetted monster so we can target them again by default next turn.
                    if (m_targettingType == TargettingType.Monster && possiblyTargettedMonster != null)
                        m_lastTargetted = possiblyTargettedMonster;

                    if (m_action != null)
                    {
                        m_action(window, engine, map.TargetPoint);
                        m_action = null;
                    }

                    Escape(map, window);
                    break;
                }
                case MagecrawlKey.Escape:
                {
                    Escape(map, window);
                    break;
                }
                case MagecrawlKey.v:
                {
                    Escape(map, window);
                    break;
                }
                case MagecrawlKey.Left:
                    HandleDirection(Direction.West, map, window, engine);
                    break;
                case MagecrawlKey.Right:
                    HandleDirection(Direction.East, map, window, engine);
                    break;
                case MagecrawlKey.Down:
                    HandleDirection(Direction.South, map, window, engine);
                    break;
                case MagecrawlKey.Up:
                    HandleDirection(Direction.North, map, window, engine);
                    break;
                case MagecrawlKey.Insert:
                    HandleDirection(Direction.Northwest, map, window, engine);
                    break;
                case MagecrawlKey.Delete:
                    HandleDirection(Direction.Southwest, map, window, engine);
                    break;
                case MagecrawlKey.PageUp:
                    HandleDirection(Direction.Northeast, map, window, engine);
                    break;
                case MagecrawlKey.PageDown:
                    HandleDirection(Direction.Southeast, map, window, engine);
                    break;
                default:
                    break;
            }
        }

        private void Escape(Map map, GameWindow window)
        {
            window.ResetDefaultKeyboardHandler();
            map.InTargettingMode = false;
            window.UpdateWorld();
        }

        private void HandleDirection(Direction direction, Map map, GameWindow window, IGameEngine engine)
        {
            if (map.InTargettingMode)
            {
                Point pointWantToGoTo = PointDirectionUtils.ConvertDirectionToDestinationPoint(map.TargetPoint, direction);
                Point resultPoint = TargetHandlerHelper.MoveSelectionToNewPoint(engine, pointWantToGoTo, direction, m_targetablePoints);
                if (resultPoint != Point.Invalid)
                    map.TargetPoint = resultPoint;
                map.Draw();
            }
            else
            {
                throw new InvalidOperationException("TargettingModeKeyboardHandler and Map disagree on current state");
            }
        }

        private Point SetTargettingInitialSpot(IGameEngine engine)
        {
            switch (m_targettingType)
            {
                case TargettingType.Monster:
                {
                    if (m_lastTargetted != null && m_lastTargetted.IsAlive && EffectivePoint.PositionInTargetablePoints(m_lastTargetted.Position, m_targetablePoints))
                        return m_lastTargetted.Position;

                    List<ICharacter> monstersInRange = new List<ICharacter>();
                    foreach (ICharacter m in engine.Map.Monsters)
                    {
                        if (EffectivePoint.PositionInTargetablePoints(m.Position, m_targetablePoints))
                            monstersInRange.Add(m);
                    }
                    ICharacter lowestHPMonster = monstersInRange.OrderBy(x => x.CurrentHP / x.MaxHP).Reverse().FirstOrDefault();
                    if (lowestHPMonster != null)
                    {
                        m_lastTargetted = lowestHPMonster;
                        return lowestHPMonster.Position;
                    }
                    break;
                }
                case TargettingType.Operatable:
                {
                    foreach (IMapObject m in engine.Map.MapObjects.Where(x => x.CanOperate))
                    {
                        if (EffectivePoint.PositionInTargetablePoints(m.Position, m_targetablePoints))
                            return m.Position;
                    }
                    break;
                }
                case TargettingType.OpenFloor:
                {
                    foreach (Direction d in DirectionUtils.GenerateRandomDirectionList())
                    {
                        Point targetLocation = PointDirectionUtils.ConvertDirectionToDestinationPoint(engine.Player.Position, d);
                        if (engine.Map.GetTerrainAt(targetLocation) == TerrainType.Wall)
                            continue;
                        if (engine.Map.Monsters.Where(x => x.Position == targetLocation).Count() > 0)
                            continue;
                        if (engine.Map.MapObjects.Where(x => x.Position == targetLocation && x.IsSolid).Count() > 0)
                            continue;
                        return targetLocation;
                    }
                    break;
                }
            }

            // If we can't find a better spot, use the player's position
            return engine.Player.Position;
        }
    }
}
