using System;
using System.Windows.Input;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

using MageCrawlPoint = Magecrawl.Utilities.Point;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public delegate void OnTargetSelect(IGameEngine engine, MageCrawlPoint point);

    public class TargettingModeKeyboardHandler
    {
        public TargettingModeKeyboardHandler()
        {
        }

        public TargettingModeKeyboardHandler(OnTargetSelect selectionAction)
        {
            m_action = selectionAction;
        }

        private OnTargetSelect m_action;

        public void OnKeyboardDown(Key key, Map map, GameWindow window, IGameEngine engine)
        {
            bool shift = Keyboard.Modifiers == ModifierKeys.Shift;
            switch (key)
            {
                case Key.Enter:
                {
                    if (m_action != null)
                    {
                        m_action(engine, map.TargetPoint);
                        m_action = null;
                    }
                    break;
                }
                case Key.Escape:
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
