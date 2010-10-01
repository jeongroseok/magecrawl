using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using MageCrawl.Silverlight.List;

using MageCrawlPoint = Magecrawl.Utilities.Point;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public class DefaultKeyboardHandler
    {
        private GameWindow m_window;
        private IGameEngine m_engine;
        private Map m_map;

        public DefaultKeyboardHandler(GameWindow window, IGameEngine engine, Map map)
        {
            m_window = window;
            m_engine = engine;
            m_map = map;
        }

        public void OnKeyboardDown(MagecrawlKey key, Map map, GameWindow window, IGameEngine engine)
        {
            switch (key)
            {
                case MagecrawlKey.Q:
                {
                    
                    break;
                }
                case MagecrawlKey.a:
                {
                    if (m_engine.Player.CurrentWeapon.IsRanged && !m_engine.Player.CurrentWeapon.IsLoaded)
                    {
                        m_engine.Actions.ReloadWeapon();
                        window.MessageBox.AddMessage(string.Format("{0} reloads the {1}.", m_engine.Player.Name, m_engine.Player.CurrentWeapon.DisplayName));
                    }
                    else
                    {
                        List<EffectivePoint> targetPoints = m_engine.GameState.CalculateTargetablePointsForEquippedWeapon();
                        OnTargetSelect selectionDelegate = (w, e, p) => 
                        {
                            if (p != m_engine.Player.Position)
                                m_engine.Actions.Attack(p);
                        };
                        TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(TargettingModeKeyboardHandler.TargettingType.Monster, engine, m_map, selectionDelegate, targetPoints);
                        window.SetKeyboardHandler(handler.OnKeyboardDown);
                    }
                    break;
                }
                case MagecrawlKey.o:
                {
                    List<EffectivePoint> operatePoints = CalculateOperatePoints();
                    TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(TargettingModeKeyboardHandler.TargettingType.Operatable, engine, m_map, (w,e,p) => m_engine.Actions.Operate(p), operatePoints);
                    window.SetKeyboardHandler(handler.OnKeyboardDown);
                    break;
                }
                case MagecrawlKey.v:
                {
                    TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(TargettingModeKeyboardHandler.TargettingType.None, engine, m_map, null);
                    m_map.UseViewCursor = true;                    
                    window.SetKeyboardHandler(handler.OnKeyboardDown);
                    break;
                }
                case MagecrawlKey.A:
                {
                    TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(TargettingModeKeyboardHandler.TargettingType.None, engine, m_map, OnRunTargetSelected, null);
                    window.SetKeyboardHandler(handler.OnKeyboardDown);
                    break;
                }
                case MagecrawlKey.i:
                {
                    ListSelection listSelection = new ListSelection(window, engine.Player.Items.OfType<INamedItem>(), "Inventory");
                    listSelection.SelectionDelegate = i =>
                    {
                        ItemSelection.OnSelection onItemSelection = (item, option) =>
                        {
                            TargetingInfo targetInfo = m_engine.Targetting.GetTargettingTypeForInventoryItem(item, option);
                            HandleInvoke(targetInfo, p => m_engine.Actions.SelectedItemOption(item, option, p), item);
                        };

                        ItemSelection selection = new ItemSelection(engine, (IItem)i, onItemSelection);
                        selection.ParentWindow = listSelection;
                        selection.Show();
                    };
                    listSelection.DismissOnSelection = false;
                    listSelection.Show();
                    break;
                }
                case MagecrawlKey.z:
                {
                    ListSelection listSelection = new ListSelection(window, engine.Player.Spells.OfType<INamedItem>(), "Spellbook");
                    listSelection.SelectionDelegate = s =>
                    {
                        TargetingInfo targetInfo = ((ISpell)s).Targeting;
                        HandleInvoke(targetInfo, p => m_engine.Actions.CastSpell((ISpell)s, p), s);
                    };

                    listSelection.Show();
                    break;
                }
                case MagecrawlKey.E:
                {
                    ListSelection listSelection = new ListSelection(window, engine.Player.StatusEffects.OfType<INamedItem>(), "Dismiss Effect");
                    listSelection.SelectionDelegate = i => engine.Actions.DismissEffect(i.DisplayName);
                    listSelection.Show();
                    break;
                }
                case MagecrawlKey.Comma:
                {
                    List<INamedItem> itemsAtLocation = engine.Map.Items.Where(i => i.Second == engine.Player.Position).Select(i => i.First).OfType<INamedItem>().ToList();
                    if (itemsAtLocation.Count > 1)
                    {
                        ListSelection listSelection = new ListSelection(window, itemsAtLocation, "Pickup Item");
                        listSelection.SelectionDelegate = i => engine.Actions.GetItem((IItem)i);
                        listSelection.Show();
                    }
                    else
                    {
                        engine.Actions.GetItem();
                        window.UpdateWorld();
                    }
                    break;
                }
                case MagecrawlKey.Backquote:
                {
                    engine.Actions.SwapPrimarySecondaryWeapons();
                    window.UpdateWorld();
                    break;
                }
                case MagecrawlKey.Period:
                {
                    engine.Actions.Wait();
                    window.UpdateWorld();
                    break;
                }
                case MagecrawlKey.PageUp:
                {
                    window.MessageBox.PageUp();
                    break;
                }
                case MagecrawlKey.PageDown:
                {
                    window.MessageBox.PageDown();
                    break;
                }
                case MagecrawlKey.Backspace:
                {
                    window.MessageBox.Clear();
                    break;
                }
                case MagecrawlKey.Left:
                    HandleDirection(Direction.West, m_map, window, engine);
                    break;
                case MagecrawlKey.Right:
                    HandleDirection(Direction.East, m_map, window, engine);
                    break;
                case MagecrawlKey.Down:
                    HandleDirection(Direction.South, m_map, window, engine);
                    break;
                case MagecrawlKey.Up:
                    HandleDirection(Direction.North, m_map, window, engine);
                    break;
                case MagecrawlKey.Insert:
                    HandleDirection(Direction.Northwest, m_map, window, engine);
                    break;
                case MagecrawlKey.Delete:
                    HandleDirection(Direction.Southwest, m_map, window, engine);
                    break;
                case MagecrawlKey.Home:
                    HandleDirection(Direction.Northeast, m_map, window, engine);
                    break;
                case MagecrawlKey.End:
                    HandleDirection(Direction.Southeast, m_map, window, engine);
                    break;
            }
        }

        private List<EffectivePoint> CalculateOperatePoints()
        {
            List<EffectivePoint> listOfSelectablePoints = new List<EffectivePoint>();

            foreach (IMapObject mapObj in m_engine.Map.MapObjects)
            {
                if (PointDirectionUtils.LatticeDistance(m_engine.Player.Position, mapObj.Position) == 1)
                {
                    // If there are any monsters at that location, don't select it.
                    if (m_engine.Map.Monsters.Where(x => x.Position == mapObj.Position).Count() == 0)
                        listOfSelectablePoints.Add(new EffectivePoint(mapObj.Position, 1.0f));
                }
            }
            return listOfSelectablePoints;
        }

        private delegate void OnTargettingPointSelected(MageCrawlPoint p);
        private void HandleInvoke(TargetingInfo targetInfo, OnTargettingPointSelected onInvoke, object invokingObject)
        {
            if (targetInfo == null)
            {
                onInvoke(m_engine.Player.Position);
                m_window.UpdateWorld();
            }
            else
            {
                switch (targetInfo.Type)
                {
                    case TargetingInfo.TargettingType.Stream:
                    {
                        List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListOneStepAllDirections(m_engine.Player.Position);
                        HandleRangedSinglePointInvoke(onInvoke, targetablePoints);
                        break;
                    }
                    case TargetingInfo.TargettingType.RangedSingle:
                    case TargetingInfo.TargettingType.RangedBlast:
                    case TargetingInfo.TargettingType.RangedExplodingPoint:
                    {
                        List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(m_engine.Player.Position, targetInfo.Range);
                        HandleRangedSinglePointInvoke(onInvoke, targetablePoints);
                        break;
                    }
                    case TargetingInfo.TargettingType.Cone:
                    {
                        MageCrawlPoint playerPosition = m_engine.Player.Position;
                        List<EffectivePoint> targetablePoints = GetConeTargetablePoints(playerPosition);
                        OnTargetSelect selectionDelegate = (w, e, p) =>
                        {
                            if (p != m_engine.Player.Position)
                                onInvoke(p);
                        };

                        TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(TargettingModeKeyboardHandler.TargettingType.Monster, m_engine, m_map, selectionDelegate, targetablePoints);
                        m_window.SetKeyboardHandler(handler.OnKeyboardDown);
                        break;
                    }
                    case TargetingInfo.TargettingType.Self:
                    {
                        onInvoke(m_engine.Player.Position);
                        m_window.UpdateWorld();
                        break;
                    }
                    default:
                        throw new System.InvalidOperationException("InvokingKeystrokeHandler - HandleInvoke, don't know how to handle: " + targetInfo.Type.ToString());
                }
            }
        }

        private void HandleRangedSinglePointInvoke(OnTargettingPointSelected selectionDelegate, List<EffectivePoint> targetablePoints)
        {
            m_engine.Targetting.FilterNotTargetableToPlayerPointsFromList(targetablePoints, true);
            m_engine.Targetting.FilterNotVisibleToPlayerBothWaysFromList(targetablePoints, true);

            OnTargetSelect onSelection = (window, engine, point) => 
                {
                    if (point != m_engine.Player.Position)
                        selectionDelegate(point);
                };

            TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(TargettingModeKeyboardHandler.TargettingType.Monster, m_engine, m_map, onSelection, targetablePoints);

            m_window.SetKeyboardHandler(handler.OnKeyboardDown);
        }

        private List<EffectivePoint> GetConeTargetablePoints(MageCrawlPoint playerPosition)
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();
            targetablePoints.Add(new EffectivePoint(playerPosition + new MageCrawlPoint(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(playerPosition + new MageCrawlPoint(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(playerPosition + new MageCrawlPoint(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(playerPosition + new MageCrawlPoint(-1, 0), 1.0f));
            m_engine.Targetting.FilterNotTargetableToPlayerPointsFromList(targetablePoints, true);
            return targetablePoints;
        }

        private static void OnRunTargetSelected(GameWindow window, IGameEngine engine, MageCrawlPoint point)
        {
            window.Map.InTargettingMode = false;
            RunningKeyboardHandler runner = new RunningKeyboardHandler(window, engine);
            runner.StartRunning(point);
        }

        private static void HandleDirection(Direction direction, Map map, GameWindow window, IGameEngine engine)
        {
            if (!map.InTargettingMode)
            {
                if (Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    engine.Actions.Move(direction);
                    window.UpdateWorld();
                }
                else
                {
                    RunningKeyboardHandler runner = new RunningKeyboardHandler(window, engine);
                    runner.StartRunning(direction);
                }
            }
            else
            {
                throw new InvalidOperationException("DefaultKeyboardHandler and Map disagree on current state");
            }
        }
    }
}
