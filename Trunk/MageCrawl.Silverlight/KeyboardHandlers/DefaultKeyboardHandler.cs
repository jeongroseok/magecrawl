using System;
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
                    ListSelection listBox = new ListSelection(window, engine.Player.Items.OfType<INamedItem>(), null, "Inventory");
                    listBox.Show();
                    break;
                }
                case MagecrawlKey.z:
                {
                    ListSelection listBox = new ListSelection(window, engine.Player.Spells.OfType<INamedItem>(), null, "Spellbook");
                    listBox.Show();
                    break;
                }
                case MagecrawlKey.E:
                {
                    ListSelection.OnSelection onSelected = i => engine.Actions.DismissEffect(i.DisplayName);
                    ListSelection listBox = new ListSelection(window, engine.Player.StatusEffects.OfType<INamedItem>(), onSelected, "Dismiss Effect");
                    listBox.Show();
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
