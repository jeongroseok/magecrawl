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
                case System.Windows.Input.Key.V:
                {
                    if (!shift)
                    {
                        map.InTargettingMode = true;
                        window.SetKeyboardHandler(TargettingModeKeyboardHandler.OnKeyboardDown);
                        window.UpdateWorld();
                    }
                    break;
                }
                case System.Windows.Input.Key.Left:
                    HandleDirection(Direction.West, map, window, engine);
                    break;
                case System.Windows.Input.Key.Right:
                    HandleDirection(Direction.East, map, window, engine);
                    break;
                case System.Windows.Input.Key.Down:
                    HandleDirection(Direction.South, map, window, engine);
                    break;
                case System.Windows.Input.Key.Up:
                    HandleDirection(Direction.North, map, window, engine);
                    break;
                case System.Windows.Input.Key.Insert:
                    HandleDirection(Direction.Northwest, map, window, engine);
                    break;
                case System.Windows.Input.Key.Delete:
                    HandleDirection(Direction.Southwest, map, window, engine);
                    break;
                case System.Windows.Input.Key.PageUp:
                    HandleDirection(Direction.Northeast, map, window, engine);
                    break;
                case System.Windows.Input.Key.PageDown:
                    HandleDirection(Direction.Southeast, map, window, engine);
                    break;
                default:
                    break;
            }
        }

        private static void HandleDirection(Direction direction, Map map, GameWindow window, IGameEngine engine)
        {
            if (!map.InTargettingMode)
            {
                engine.Actions.Move(direction);
                window.UpdateWorld();
            }
            else
            {
                throw new InvalidOperationException("DefaultKeyboardHandler and Map disagree on current state");
            }
        }
    }
}
