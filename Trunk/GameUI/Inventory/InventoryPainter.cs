using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Inventory
{
    public delegate void InventoryItemSelected(IItem item);

    // This code is scary, I admit it. It looks complex, but it has to be. 
    // Scrolling inventory right, when it might be lettered, is hard.
    internal class InventoryPainter : PainterBase
    {
        private bool m_enabled;                         // Are we showing the inventory
        private IList<IItem> m_itemList;                // Items to display
        private int m_lowerRange;                       // If we're scrolling, the loweset number item to show
        private int m_higherRange;                      // Last item to show
        private bool m_isScrollingNeeded;               // Do we need to scroll at all?
        private int m_cursorPosition;                   // What item is the cursor on
        private bool m_useCharactersNextToItems;        // Should we put letters next to each letter
        private bool m_shouldNotResetCursorPosition;    // If set, the next time we show the inventory window, we don't reset the position.

        private DialogColorHelper m_dialogColorHelper;

        private const int ScrollAmount = 8;
        private const int InventoryWindowOffset = 5;
        private const int InventoryItemWidth = UIHelper.ScreenWidth - 10;
        private const int InventoryItemHeight = UIHelper.ScreenHeight - 10;
        private const int NumberOfLinesDisplayable = InventoryItemHeight - 2;

        internal InventoryPainter()
        {
            m_dialogColorHelper = new DialogColorHelper();
            m_enabled = false;
            m_shouldNotResetCursorPosition = false;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
            m_shouldNotResetCursorPosition = false;

            if (m_enabled)
            {
                m_itemList = engine.Player.Items;
                m_isScrollingNeeded = m_itemList.Count > NumberOfLinesDisplayable;
                
                // If we're going to run out of letters, don't show em.
                if (m_itemList.Count > 26 * 2)
                    m_useCharactersNextToItems = false;
                else
                    m_useCharactersNextToItems = true;
            }
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                m_higherRange = m_isScrollingNeeded ? m_lowerRange + NumberOfLinesDisplayable : m_itemList.Count;
                screen.DrawFrame(InventoryWindowOffset, InventoryWindowOffset, InventoryItemWidth, InventoryItemHeight, true, "Inventory");
                
                // Start lettering from our placementOffset.
                char currentLetter = 'a';

                if (m_useCharactersNextToItems)
                {
                    for (int i = 0; i < m_lowerRange; ++i)
                        currentLetter = IncrementLetter(currentLetter);
                }

                int positionalOffsetFromTop = 0;
                m_dialogColorHelper.SaveColors(screen);
                for (int i = m_lowerRange; i < m_higherRange; ++i)
                {
                    string displayString = m_itemList[i].Name;
                    m_dialogColorHelper.SetColors(screen, i == m_cursorPosition, true);
                    if (displayString.Contains('\t'.ToString()))
                    {
                        // This is the case for Tab Seperated Spaces, used for magic lists and such
                        throw new System.NotImplementedException();
                    }
                    else
                    {
                        string printString;
                        if (m_useCharactersNextToItems)
                            printString = string.Format("{0} - {1}", currentLetter, displayString);
                        else
                            printString = "  " + displayString;
                        screen.PrintLine(printString, InventoryWindowOffset + 1, InventoryWindowOffset + 1 + positionalOffsetFromTop, Background.Set, LineAlignment.Left);
                    }

                    currentLetter = IncrementLetter(currentLetter);
                    positionalOffsetFromTop++;
                }
                m_dialogColorHelper.ResetColors(screen);
            }
        }

        public override void HandleRequest(string request, object data, object data2)
        {
            switch (request)
            {
                case "InventoryWindowSavePosition":
                    m_shouldNotResetCursorPosition = true;
                    break;
                case "ShowInventoryWindow":
                    if (!m_shouldNotResetCursorPosition)
                    {
                        m_cursorPosition = 0;
                        m_lowerRange = 0;
                        m_higherRange = 0;
                    }
                    else
                    {
                        m_shouldNotResetCursorPosition = false;
                    }
                    m_enabled = true;
                    break;
                case "StopShowingInventoryWindow":
                    m_enabled = false;
                    break;
                case "IntentoryItemSelected":
                {
                        InventoryItemSelected del = (InventoryItemSelected)data;
                        del(m_itemList[m_cursorPosition]);
                        break;
                }
                case "IntentoryItemSelectedByChar":
                {
                    InventoryItemSelected del = (InventoryItemSelected)data;
                    if (m_useCharactersNextToItems)
                    {
                        char selectedLetter = (char)data2;
                        List<char> listOfLettersUsed = GetListOfLettersUsed();
                        if (listOfLettersUsed.Contains(selectedLetter))
                        {
                            m_cursorPosition = listOfLettersUsed.IndexOf(selectedLetter);

                            del(m_itemList[m_cursorPosition]);
                        }
                    }
                    break;
                }
                case "InventoryPositionChanged":
                {
                    MoveInventorySelection((Direction)data);
                    break;
                }
            }
        }

        private List<char> GetListOfLettersUsed()
        {
            if (!m_useCharactersNextToItems)
                throw new System.ArgumentException("GetListOfLettersUsed can't be called when not using letters next to names");

            List<char> returnList = new List<char>();
            char elt = 'a';
            while (elt != MapSelectionOffsetToLetter(m_itemList.Count))
            {
                returnList.Add(elt);
                elt = IncrementLetter(elt);
            }
            return returnList;
        }

        private static char MapSelectionOffsetToLetter(int offset)
        {
            if (offset > 25)
                return (char)('A' + (char)(offset - 26));
            else
                return (char)('a' + (char)offset);
        }

        private static char IncrementLetter(char letter)
        {
            if (letter == 'Z')
                return 'a';
            else if (letter == 'z')
                return 'A';
            else
                return (char)(((int)letter) + 1);
        }

        private void MoveInventorySelection(Direction cursorDirection)
        {
            if (cursorDirection == Direction.North)
            {
                if (m_cursorPosition > 0)
                {
                    if (m_isScrollingNeeded && (m_cursorPosition == m_lowerRange))
                    {
                        m_lowerRange -= ScrollAmount;
                        if (m_lowerRange < 0)
                            m_lowerRange = 0;
                    }
                    m_cursorPosition--;
                }
            }
            if (cursorDirection == Direction.South && m_cursorPosition < m_itemList.Count - 1)
            {
                // If we need scrolling and we're pointed at the end of the list and there's more to show.
                if (m_isScrollingNeeded && (m_cursorPosition == (m_lowerRange - 1 + NumberOfLinesDisplayable)) && (m_lowerRange + NumberOfLinesDisplayable < m_itemList.Count))
                {
                    m_lowerRange += ScrollAmount;
                    if ((m_lowerRange + NumberOfLinesDisplayable) > m_itemList.Count)
                        m_lowerRange = m_itemList.Count - NumberOfLinesDisplayable;

                    m_cursorPosition++;
                }
                else
                {
                    if ((m_cursorPosition + 1) < m_itemList.Count)
                        m_cursorPosition++;
                }
            }
        }
    }
}
