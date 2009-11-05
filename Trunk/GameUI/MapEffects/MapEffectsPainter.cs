using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameUI.MapEffects
{
    public delegate void EffectDone();

    internal sealed class MapEffectsPainter : MapPainterBase
    {
        private const int millisecondsPerFrame = 50;

        private enum EffectTypes { None, RangedBolt };
        
        private uint m_animationStartTime;

        private Point m_mapUpCorner;
        
        private List<Point> m_path;
        private EffectTypes m_type;
        private Color m_color;
        
        private EffectDone m_done;

        public MapEffectsPainter() : base() 
        {
            m_done = null;
        }

        public override void DrawNewFrame(Console screen)
        {
            switch (m_type)
            {
                case EffectTypes.RangedBolt:
                {
                    uint frameNumber = (TCODSystem.ElapsedMilliseconds - m_animationStartTime) / millisecondsPerFrame;
                    if (m_path.Count <= frameNumber)
                    {
                        m_done();
                        m_done = null;
                        m_type = EffectTypes.None;
                        return;
                    }
                    else
                    {
                        Point boltPoint = m_path[(int)frameNumber];
                        Point screenPosition = new Point(m_mapUpCorner.X + boltPoint.X + 1, m_mapUpCorner.Y + boltPoint.Y + 1);
                        screen.PutChar(screenPosition.X , screenPosition.Y, '*');
                        screen.SetCharForeground(screenPosition.X, screenPosition.Y, m_color);
                    }
                    return;
                }
                case EffectTypes.None:
                {
                    return;
                }
            }
        }

        public void DrawRangedBolt(EffectDone effectDoneDelegate, List<Point> path, Color color)
        {
            m_type = EffectTypes.RangedBolt;
            m_animationStartTime = TCODSystem.ElapsedMilliseconds;
            m_done = effectDoneDelegate;
            m_path = path;
            m_color = color;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            m_mapUpCorner = mapUpCorner;
        }

        public override void Dispose()
        {
        }
    }
}
