using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public sealed class TextBox
    {
        private const int MaxNumberOfLines = 500;
        private List<string> m_textList;
        private int m_textBoxPosition;

        public TextBox()
        {
            m_textList = new List<string>();
            m_textBoxPosition = 0;
        }

        public void Draw(RootConsole screen)
        {
            const int TextBoxYPosition = 43;
            const int TextBoxWidth = 51;
            const int TextBoxHeight = 17;
            const int LinesInTextConsole = TextBoxHeight - 2;
            screen.DrawFrame(0, TextBoxYPosition, TextBoxWidth, TextBoxHeight, true);

            if (m_textList.Count == 0)
                return;        // No Text == Nothing to Draw.

            // Draw either all the text, or all that will fit
            int numberToDraw = ((m_textList.Count - m_textBoxPosition) < LinesInTextConsole) ?
                (m_textList.Count - m_textBoxPosition) : LinesInTextConsole;

            int currentElement = 0;
            for (int i = 0; i < LinesInTextConsole; ++i)
            {
                if (currentElement == numberToDraw)
                    break;
                
                string currentString = m_textList[m_textBoxPosition + currentElement];
                
                // If it's over 1 line long, we treat it special. See if it will be early.
                int numberOfLinesLong = screen.GetHeightPrintLineRectWouldUse(currentString, 1, TextBoxYPosition, TextBoxWidth - 2, 8, LineAlignment.Left);

                if (numberOfLinesLong > 1)
                {
                    i += numberOfLinesLong - 1;
                    if (i >= LinesInTextConsole)
                        break;
                    screen.PrintLineRect(currentString, 1, TextBoxYPosition + LinesInTextConsole - i, TextBoxWidth - 2, numberOfLinesLong, LineAlignment.Left);
                }
                else
                {
                    screen.PrintLine(currentString, 1, TextBoxYPosition + LinesInTextConsole - i, LineAlignment.Left);
                }
                currentElement++;
            }
        }

        public void Clear()
        {
            m_textList.Clear();
            m_textBoxPosition = 0;
        }

        public void TextBoxScrollDown()
        {
            if (m_textBoxPosition > 0)
                m_textBoxPosition--;
        }

        public void TextBoxScrollUp()
        {
            if (m_textBoxPosition < m_textList.Count - 1)
                m_textBoxPosition++;
        }

        public void TextInputFromEngineDelegate(string s)
        {
            m_textBoxPosition = 0;     // Snap to current if scrolled
            m_textList.Insert(0, s);
            while (m_textList.Count > MaxNumberOfLines)
                m_textList.RemoveAt(m_textList.Count - 1);
        }
    }
}