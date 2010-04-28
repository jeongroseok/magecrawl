using System.Collections.Generic;
using libtcod;
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

        public override void DrawNewFrame(TCODConsole screen)
        {
            if (m_isSelectionCursor)
            {
                screen.setCharBackground(ScreenCenter.X + 1, ScreenCenter.Y + 1, TCODColor.darkYellow);

                if (ToolTipsEnabled)
                {
                    if (TCODSystem.getElapsedMilli() - m_lastCursorMovedTime > TimeUntilToolTipPopup)
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

                            screen.setBackgroundColor(ColorPresets.DarkGray);

                            int frameHeight = m_currentToolTips.Count > MaxNumberOfLinesToShow ? 3 + numberOfLinesToShow : 2 + numberOfLinesToShow;
                            screen.printFrame(ScreenCenter.X + 2, ScreenCenter.Y - 2, longestLine + 2, frameHeight, false, TCODBackgroundFlag.Multiply);

                            for (int i = 0; i < numberOfLinesToShow; ++i)
                                screen.printEx(ScreenCenter.X + 3, ScreenCenter.Y - 1 + i, TCODBackgroundFlag.Multiply, TCODAlignment.LeftAlignment, m_currentToolTips[i]);

                            if (m_currentToolTips.Count > MaxNumberOfLinesToShow)
                                screen.printEx(ScreenCenter.X + 3, ScreenCenter.Y - 1 + MaxNumberOfLinesToShow, TCODBackgroundFlag.Multiply, TCODAlignment.LeftAlignment, "...more...");

                            screen.setBackgroundColor(ColorPresets.Black);
                        }
                    }
                }
            }
        }

        public void NewCursorPosition()
        {
            m_lastCursorMovedTime = TCODSystem.getElapsedMilli();
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
