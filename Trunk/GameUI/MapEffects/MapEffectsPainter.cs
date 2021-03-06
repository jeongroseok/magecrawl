﻿using System;
using System.Collections.Generic;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.MapEffects
{
    internal sealed class MapEffectsPainter : MapPainterBase
    {
        private const int MillisecondsPerFrame = 65;
        
        private static Random s_random = new Random();

        private enum EffectTypes 
        {
            None, RangedBolt, Cone, ExploadingPoint, Stream
        }
        
        private uint m_animationStartTime;

        private Point m_mapUpCorner;
        private EffectTypes m_type;
        private TCODColor m_color;
        private bool m_done;

        private List<Point> m_points;

        // For Stream
        private Dictionary<Point, bool> m_locationsOccupied;

        // For RangedBolt
        private int m_tailLength;

        // For ExploadingPoints
        private List<Point> m_path;
        private List<List<Point>> m_blast;

        // Used when we're responding to a callback from the game engine that something happened on someone else's turn.
        public void DrawAnimationSynchronous(PaintingCoordinator coord, TCODConsole screen)
        {
            while (!m_done)
            {
                coord.DrawNewFrame(screen);
                TCODConsole.flush();
            }
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            uint frameNumber = (TCODSystem.getElapsedMilli() - m_animationStartTime) / MillisecondsPerFrame;
            switch (m_type)
            {
                case EffectTypes.RangedBolt:
                    DrawRangedBoltFrame(screen, frameNumber);
                    return;
                case EffectTypes.Cone:
                    DrawConeFrame(screen, frameNumber);
                    return;
                case EffectTypes.ExploadingPoint:
                    DrawExploadingPointFrame(screen, frameNumber);
                    return;
                case EffectTypes.Stream:
                    DrawStream(screen, frameNumber);
                    return;
                case EffectTypes.None:
                default:
                    return;
            }
        }

        private void DrawStream(TCODConsole screen, uint frameNumber)
        {
            if ((m_points.Count + 8) <= frameNumber)
            {
                FinishAnimation();
            }
            else
            {
                int endingFrame = (int)System.Math.Min(m_points.Count - 1, frameNumber);

                // If we've drawn out the entire stream, show it flickering
                if (endingFrame == m_points.Count - 1)
                {
                    for (int i = 0; i <= endingFrame; ++i)
                    {
                        if (m_locationsOccupied[m_points[i]] || s_random.Chance(60))
                            DrawPoint(screen, m_points[i], '*');
                    }
                }
                else
                {
                    // Else just draw the part of the stream
                    for (int i = 0; i <= endingFrame; ++i)
                        DrawPoint(screen, m_points[i], '*');
                }                
            }
        }

        private void DrawExploadingPointFrame(TCODConsole screen, uint frameNumber)
        {
            if ((m_path.Count + (m_blast.Count * 2) - 1) < frameNumber)
            {
                FinishAnimation();
            }
            else
            {
                if (frameNumber < m_path.Count)
                {                    
                    DrawPoint(screen,  m_path[(int)frameNumber], '*');
                }
                else
                {
                    int explosionPosition = (int)(frameNumber - m_path.Count);

                    // Each "blast" frame last twice as long, as set odd frames back one
                    if ((explosionPosition % 2) == 1)
                        explosionPosition--;

                    for (int i = 0; i <= explosionPosition; ++i)
                    {
                        foreach (Point p in m_blast[i / 2])
                        {
                            DrawPoint(screen, p, '*');
                        }
                    }
                }
            }
        }

        private void DrawRangedBoltFrame(TCODConsole screen, uint frameNumber)
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
                    DrawPoint(screen, m_points[i], '*');
            }
        }

        private void DrawConeFrame(TCODConsole screen, uint frameNumber)
        {
            if (frameNumber > 9)
            {
                FinishAnimation();
            }
            else
            {
                foreach (Point p in m_points)
                {
                    if (s_random.Chance(27))
                        DrawPoint(screen, p, '#');
                }
            }
        }

        private void DrawPoint(TCODConsole screen, Point p, char c)
        {
            Point screenPosition = new Point(m_mapUpCorner.X + p.X + 1, m_mapUpCorner.Y + p.Y + 1);
            screen.putChar(screenPosition.X, screenPosition.Y, c, TCODBackgroundFlag.None);
            screen.setCharForeground(screenPosition.X, screenPosition.Y, m_color);
        }

        private void FinishAnimation()
        {
            m_done = true;

            m_type = EffectTypes.None;
            return;
        }

        public void DrawRangedBolt(List<Point> path, TCODColor color, int tailLength, bool drawLastTargetSquare)
        {
            m_type = EffectTypes.RangedBolt;
            m_animationStartTime = TCODSystem.getElapsedMilli();
            m_points = path;

            if (!drawLastTargetSquare)
                m_points.RemoveAt(m_points.Count - 1);

            m_color = color;

            m_tailLength = tailLength;

            m_done = false;
        }

        public void DrawStream(List<Point> path, TCODColor color, Dictionary<Point, bool> locationsOccupied)
        {
            m_type = EffectTypes.Stream;
            m_animationStartTime = TCODSystem.getElapsedMilli();
            m_points = path;
            m_color = color;
            m_locationsOccupied = locationsOccupied;
            m_done = false;
        }

        public void DrawConeBlast(List<Point> points, TCODColor color)
        {
            m_type = EffectTypes.Cone;
            m_animationStartTime = TCODSystem.getElapsedMilli();
            m_points = points;

            m_color = color;
            m_done = false;
        }

        public void DrawExploadingPointBlast(List<Point> path, List<List<Point>> blast, TCODColor color)
        {
            m_type = EffectTypes.ExploadingPoint;
            m_animationStartTime = TCODSystem.getElapsedMilli();
            m_path = path;
            m_blast = blast;

            m_color = color;
            m_done = false;
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            m_mapUpCorner = mapUpCorner;
        }
    }
}
