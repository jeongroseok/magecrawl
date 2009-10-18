using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public sealed class TextBox
    {
        private const int maxNumberOfLines = 500;
        private List<string> m_textList;
        private int m_textBoxPosition;

        public TextBox()
        {
            m_textList = new List<string>();
            m_textBoxPosition = 0;
        }

        public void Draw(RootConsole screen)
        {
            const int textBoxYPosition = 43;
            const int textBoxWidth = 51;
            const int textBoxHeight = 17;
            const int linesInTextConsole = textBoxHeight - 2;
            screen.DrawFrame(0, textBoxYPosition, textBoxWidth, textBoxHeight, true);

            if (m_textList.Count == 0)
                return;        // No Text == Nothing to Draw.

            // Draw either all the text, or all that will fit
            int numberToDraw = ((m_textList.Count - m_textBoxPosition) < linesInTextConsole) ?
                (m_textList.Count - m_textBoxPosition) : linesInTextConsole;

            int currentElement = 0;
            for (int i = 0; i < linesInTextConsole; ++i)
            {
                if (currentElement == numberToDraw)
                    break;
                
                string currentString = m_textList[m_textBoxPosition + currentElement];
                
                // If it's over 1 line long, we treat it special. See if it will be early.
                int numberOfLinesLong = screen.GetHeightPrintLineRectWouldUse(currentString, 1, textBoxYPosition, textBoxWidth - 2, 8, LineAlignment.Left);

                if (numberOfLinesLong > 1)
                {
                    i += numberOfLinesLong - 1;
                    if (i >= linesInTextConsole)
                        break;
                    screen.PrintLineRect(currentString, 1, textBoxYPosition + linesInTextConsole - i, textBoxWidth - 2, numberOfLinesLong, LineAlignment.Left);
                }
                else
                {
                    screen.PrintLine(currentString, 1, textBoxYPosition + linesInTextConsole - i, LineAlignment.Left);
                }
                currentElement++;
            }
        }

        public void Clear()
        {
            m_textList.Clear();
            m_textBoxPosition = 0;
        }

        public void textBoxScrollDown()
        {
            if (m_textBoxPosition > 0)
                m_textBoxPosition--;
        }

        public void textBoxScrollUp()
        {
            if (m_textBoxPosition < m_textList.Count-1)
                m_textBoxPosition++;
        }

        public void TextInputFromEngineDelegate(string s)
        {
            m_textBoxPosition = 0;     //Snap to current if scrolled
            m_textList.Insert(0, s);
            while (m_textList.Count > maxNumberOfLines)
                m_textList.RemoveAt(m_textList.Count - 1);
        }
    }
}