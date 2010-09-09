using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using Magecrawl;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public class RunningKeyboardHandler
    {
        private object m_lock;
        private IGameEngine m_engine;
        private Point m_target;
        private DispatcherTimer m_dispatcherTimer;
        private GameWindow m_window;

        public RunningKeyboardHandler(GameWindow window, IGameEngine engine)
        {
            m_window = window;
            m_engine = engine;
            m_lock = new object();
            m_dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        }

        public void StartRunning(Point target)
        {
            m_target = target;

            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150); // 150 Milliseconds
            m_dispatcherTimer.Tick += OnTick;
            m_dispatcherTimer.Start();

            m_window.SetKeyboardHandler(OnKeyboardDown);
        }

        private void OnTick(object sender, EventArgs e)
        {
            lock (m_lock)
            {
                if (!m_engine.GameState.DangerInLOS())
                {
                    List<Point> pathToPoint = m_engine.Targetting.PlayerPathToPoint(m_target);
                    if (pathToPoint == null || pathToPoint.Count == 0)
                    {
                        Escape();
                        return;
                    }

                    Direction d = PointDirectionUtils.ConvertTwoPointsToDirection(m_engine.Player.Position, pathToPoint[0]);

                    bool ableToMove = m_engine.Actions.Move(d);
                    if (!ableToMove)
                    {
                        Escape();
                        return;
                    }
                    else
                    {
                        m_window.Map.Draw();
                    }
                }
                else
                {
                    Escape();
                    return;
                }
            }
        }

        private void Escape()
        {
            lock (m_lock)
            {
                m_target = Point.Invalid;
                m_dispatcherTimer.Tick -= OnTick;
                m_window.ResetDefaultKeyboardHandler();
            }
        }

        private void OnKeyboardDown(Key key, Map map, GameWindow window, IGameEngine engine)
        {
            Escape();
        }
    }
}
