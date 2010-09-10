using System;
using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public static class DefaultKeyboardHandler
    {
        public static void OnKeyboardDown(Key key, Map map, GameWindow window, IGameEngine engine)
        {
            bool shift = Keyboard.Modifiers == ModifierKeys.Shift;
            switch (key)
            {
                case Key.V:
                {
                    if (!shift)
                    {
                        map.InTargettingMode = true;
                        TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler();
                        window.SetKeyboardHandler(handler.OnKeyboardDown);
                    }
                    break;
                }
                case Key.A:
                {
                    if (shift)
                    {
                        map.InTargettingMode = true;
                        TargettingModeKeyboardHandler handler = new TargettingModeKeyboardHandler(OnRunTargetSelected);
                        window.SetKeyboardHandler(handler.OnKeyboardDown);
                    }
                    break;
                }
                case Key.Left:
                    HandleDirection(Direction.West, map, window, engine);
                    break;
                case Key.Right:
                    HandleDirection(Direction.East, map, window, engine);
                    break;
                case Key.Down:
                    HandleDirection(Direction.South, map, window, engine);
                    break;
                case Key.Up:
                    HandleDirection(Direction.North, map, window, engine);
                    break;
                case Key.Insert:
                    HandleDirection(Direction.Northwest, map, window, engine);
                    break;
                case Key.Delete:
                    HandleDirection(Direction.Southwest, map, window, engine);
                    break;
                case Key.PageUp:
                    HandleDirection(Direction.Northeast, map, window, engine);
                    break;
                case Key.PageDown:
                    HandleDirection(Direction.Southeast, map, window, engine);
                    break;
                default:
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
