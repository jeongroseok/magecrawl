using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map
{
    internal class MapCursorPainter : MapPainterBase
    {
        private bool m_isSelectionCursor;
        private double m_lastCursorMovedTime;
        private const double TimeUntilToolTipPopup = 700; // in ms
        private List<string> m_currentToolTips;

        public MapCursorPainter()
        {
            m_isSelectionCursor = false;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            if (m_isSelectionCursor)
            {
                m_currentToolTips = engine.GetDescriptionForTile(cursorPosition);
            }
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_isSelectionCursor)
            {
                screen.SetCharBackground(ScreenCenter.X + 1, ScreenCenter.Y + 1, TCODColorPresets.DarkYellow);

                if (ToolTipsEnabled)
                {
                    if (TCODSystem.ElapsedMilliseconds - m_lastCursorMovedTime > TimeUntilToolTipPopup)
                    {
                        if (m_currentToolTips.Count > 0)
                        {
                            const int MaxNumberOfLinesToShow = 3;
                            int numberOfLinesToShow = System.Math.Min(m_currentToolTips.Count, MaxNumberOfLinesToShow);

                            int longestLine = 0;
                            for (int i = 0; i < numberOfLinesToShow; ++i)
                                longestLine = System.Math.Max(longestLine, m_currentToolTips[i].Length);

                            // If we're going to need to print "...more..." make sure we have the width
                            if (m_currentToolTips.Count > MaxNumberOfLinesToShow)
                                longestLine = System.Math.Max(longestLine, 10);

                            screen.BackgroundColor = TCODColorPresets.DarkGray;

                            int frameHeight = m_currentToolTips.Count > MaxNumberOfLinesToShow ? 3 + numberOfLinesToShow : 2 + numberOfLinesToShow;
                            screen.DrawFrame(ScreenCenter.X + 2, ScreenCenter.Y - 2, longestLine + 2, frameHeight, false, new Background(BackgroundFlag.Multiply));

                            for (int i = 0; i < numberOfLinesToShow; ++i)
                                screen.PrintLine(m_currentToolTips[i], ScreenCenter.X + 3, ScreenCenter.Y - 1 + i, new Background(BackgroundFlag.Multiply), LineAlignment.Left);

                            if (m_currentToolTips.Count > MaxNumberOfLinesToShow)
                                screen.PrintLine("...more...", ScreenCenter.X + 3, ScreenCenter.Y - 1 + MaxNumberOfLinesToShow, new Background(BackgroundFlag.Multiply), LineAlignment.Left);

                            screen.BackgroundColor = TCODColorPresets.Black;
                        }
                    }
                }
            }
        }

        public void NewCursorPosition()
        {
            m_lastCursorMovedTime = TCODSystem.ElapsedMilliseconds;
        }

        internal override void DisableAllOverlays()
        {
            m_isSelectionCursor = false;
            ToolTipsEnabled = false;
        }

        internal bool MapCursorEnabled
        {
            get 
            {
                return m_isSelectionCursor; 
            }
            set
            {
                m_isSelectionCursor = value;
                NewCursorPosition();
            }
        }

        internal bool ToolTipsEnabled { get; set; }
    }
}
