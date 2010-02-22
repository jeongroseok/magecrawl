using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.MapEffects
{
    public delegate void EffectDone();

    internal sealed class MapEffectsPainter : MapPainterBase
    {
        private const int MillisecondsPerFrame = 60;
        
        private static TCODRandom m_random = new TCODRandom();

        private enum EffectTypes 
        {
            None, RangedBolt, Cone
        }
        
        private uint m_animationStartTime;

        private Point m_mapUpCorner;
        private EffectTypes m_type;
        private Color m_color;
        private bool m_done;
        private EffectDone m_doneDelegate;

        private List<Point> m_points;

        // For RangedBolt
        private int m_tailLength;

        public MapEffectsPainter() : base() 
        {
            m_doneDelegate = null;
        }

        // Used when we're responding to a callback from the game engine that something happened on someone else's turn.
        public void DrawAnimationSynchronous(PaintingCoordinator coord, RootConsole screen)
        {
            while (!m_done)
            {
                coord.DrawNewFrame(screen);
                screen.Flush();
            }
        }

        public override void DrawNewFrame(Console screen)
        {
            uint frameNumber = (TCODSystem.ElapsedMilliseconds - m_animationStartTime) / MillisecondsPerFrame;
            switch (m_type)
            {
                case EffectTypes.RangedBolt:
                    DrawRangedBoltFrame(screen, frameNumber);
                    return;
                case EffectTypes.Cone:
                    DrawConeFrame(screen, frameNumber);
                    return;
                case EffectTypes.None:
                default:
                    return;
            }
        }

        private void DrawRangedBoltFrame(Console screen, uint frameNumber)
        {
            if ((m_points.Count + m_tailLength) <= frameNumber)
            {
                FinishAnimation();
            }
            else
            {
                int startingFrame = (int)System.Math.Max(frameNumber - m_tailLength, 0);
                int endingFrame = (int)System.Math.Min(m_points.Count - 1, frameNumber);
                for (int i = startingFrame; i <= endingFrame; ++i)
                {
                    Point boltPoint = m_points[i];
                    Point screenPosition = new Point(m_mapUpCorner.X + boltPoint.X + 1, m_mapUpCorner.Y + boltPoint.Y + 1);
                    screen.PutChar(screenPosition.X, screenPosition.Y, '*', Background.None);
                    screen.SetCharForeground(screenPosition.X, screenPosition.Y, m_color);
                }
            }
        }

        private void DrawConeFrame(Console screen, uint frameNumber)
        {
            if (frameNumber > 9)
            {
                FinishAnimation();
            }
            else
            {
                foreach(Point p in m_points)
                {
                    if (m_random.Chance(27))
                    {
                        Point screenPosition = new Point(m_mapUpCorner.X + p.X + 1, m_mapUpCorner.Y + p.Y + 1);
                        screen.PutChar(screenPosition.X, screenPosition.Y, '#', Background.None);
                        screen.SetCharForeground(screenPosition.X, screenPosition.Y, m_color);
                    }
                }
            }
        }

        private void FinishAnimation()
        {
            m_done = true;
            
            if (m_doneDelegate != null)
                m_doneDelegate();

            m_doneDelegate = null;
            m_type = EffectTypes.None;
            return;
        }

        public void DrawRangedBolt(EffectDone effectDoneDelegate, List<Point> path, Color color, int tailLength, bool drawLastTargetSquare)
        {
            m_type = EffectTypes.RangedBolt;
            m_animationStartTime = TCODSystem.ElapsedMilliseconds;
            m_doneDelegate = effectDoneDelegate;
            m_points = path;

            if (!drawLastTargetSquare)
                m_points.RemoveAt(m_points.Count - 1);

            m_color = color;
            m_tailLength = tailLength;
            m_done = false;
        }

        public void DrawConeBlast(EffectDone effectDoneDelegate, List<Point> points, Color color)
        {
            m_type = EffectTypes.Cone;
            m_animationStartTime = TCODSystem.ElapsedMilliseconds;
            m_doneDelegate = effectDoneDelegate;
            m_points = points;

            m_color = color;
            m_done = false;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            m_mapUpCorner = mapUpCorner;
        }
    }
}
