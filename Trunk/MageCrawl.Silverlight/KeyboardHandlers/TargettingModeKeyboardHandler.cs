using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using System;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public class TargettingModeKeyboardHandler
    {
        public static void OnKeyboardDown(Key key, Map map, GameWindow window, IGameEngine engine)
        {
            bool shift = Keyboard.Modifiers == ModifierKeys.Shift;
            switch (key)
            {
                case System.Windows.Input.Key.Escape:
                {
                    Escape(map, window);
                    break;
                }
                case Key.V:
                {
                    if (!shift)
                        Escape(map, window);
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

        private static void Escape(Map map, GameWindow window)
        {
            window.ResetDefaultKeyboardHandler();
            map.InTargettingMode = false;
            window.UpdateWorld();
        }

        private static void HandleDirection(Direction direction, Map map, GameWindow window, IGameEngine engine)
        {
            if (map.InTargettingMode)
            {
                map.TargetPoint = PointDirectionUtils.ConvertDirectionToDestinationPoint(map.TargetPoint, direction);
                map.Draw();
            }
            else
            {
                throw new InvalidOperationException("TargettingModeKeyboardHandler and Map disagree on current state");
            }
        }
    }
}
