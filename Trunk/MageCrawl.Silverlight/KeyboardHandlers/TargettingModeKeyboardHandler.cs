using System;
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
        public TargettingModeKeyboardHandler()
        {
        }

        public TargettingModeKeyboardHandler(OnTargetSelect selectionAction)
        {
            m_action = selectionAction;
        }

        private OnTargetSelect m_action;

        public void OnKeyboardDown(MagecrawlKey key, Map map, GameWindow window, IGameEngine engine)
        {
            switch (key)
            {
                case MagecrawlKey.Enter:
                {
                    if (m_action != null)
                    {
                        m_action(window, engine, map.TargetPoint);
                        m_action = null;
                    }
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
