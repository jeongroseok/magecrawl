using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using MageCrawl.Silverlight.List;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public static class DefaultKeyboardHandler
    {
        public static void OnKeyboardDown(MagecrawlKey key, Map map, GameWindow window, IGameEngine engine)
        {
            switch (key)
            {
                case MagecrawlKey.v:
                {
                    map.InTargettingMode = true;
                    TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler();
                    window.SetKeyboardHandler(handler.OnKeyboardDown);
                    break;
                }
                case MagecrawlKey.A:
                {
                    map.InTargettingMode = true;
                    TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(OnRunTargetSelected);
                    window.SetKeyboardHandler(handler.OnKeyboardDown);
                    break;
                }
                case MagecrawlKey.i:
                {
                    ListSelection listSelection = new ListSelection(window, engine.Player.Items.OfType<INamedItem>(), "Inventory");
                    listSelection.SelectionDelegate = i =>
                    {
                        ItemSelection selection = new ItemSelection((IItem)i);
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
                case MagecrawlKey.Home:
                    HandleDirection(Direction.Northeast, map, window, engine);
                    break;
                case MagecrawlKey.End:
                    HandleDirection(Direction.Southeast, map, window, engine);
                    break;
            }
        }

        private static void OnRunTargetSelected(GameWindow window, IGameEngine engine, Point point)
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
