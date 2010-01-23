using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.MapEffects
{
    public delegate void EffectDone();

    internal sealed class MapEffectsPainter : MapPainterBase
    {
        private const int MillisecondsPerFrame = 500;

        private enum EffectTypes 
        {
            None, RangedBolt
        }
        
        private uint m_animationStartTime;

        private Point m_mapUpCorner;
        private EffectTypes m_type;
        private Color m_color;
        private bool m_done;
        private EffectDone m_doneDelegate;

        // For RangedBolt
        private List<Point> m_path;
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
                {
                    DrawRangedBoltFrame(screen, frameNumber, m_tailLength);
                    return;
                }
                case EffectTypes.None:
                {
                    return;
                }
            }
        }

        private void DrawRangedBoltFrame(Console screen, uint frameNumber, int sizeOfTail)
        {
            if ((m_path.Count + sizeOfTail) <= frameNumber)
            {
                FinishAnimation();
            }
            else
            {
                int startingFrame = (int)System.Math.Max(frameNumber - sizeOfTail, 0);
                int endingFrame = (int)System.Math.Min(m_path.Count-1, frameNumber);
                for (int i = startingFrame; i <= endingFrame; ++i)
                {
                    Point boltPoint = m_path[i];
                    Point screenPosition = new Point(m_mapUpCorner.X + boltPoint.X + 1, m_mapUpCorner.Y + boltPoint.Y + 1);
                    screen.PutChar(screenPosition.X, screenPosition.Y, '*', Background.None);
                    screen.SetCharForeground(screenPosition.X, screenPosition.Y, m_color);
                }
            }
        }

        private void FinishAnimation()
        {
            m_done = true;
            
            if(m_doneDelegate != null)
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
            m_path = path;

            if(!drawLastTargetSquare)
                m_path.RemoveAt(m_path.Count - 1);

            m_color = color;
            m_tailLength = tailLength;
            m_done = false;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            m_mapUpCorner = mapUpCorner;
        }
    }
}
