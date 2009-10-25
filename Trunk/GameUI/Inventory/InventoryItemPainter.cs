using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Inventory
{
    public delegate void InventoryItemOptionSelected(IItem item, string optionName);

    internal sealed class InventoryItemPainter : PainterBase
    {
        private const int SelectedItemOffset = 8;
        private const int SelectedItemWidth = UIHelper.ScreenWidth - (SelectedItemOffset * 2);
        private const int SelectedItemHeight = UIHelper.ScreenHeight - (SelectedItemOffset * 2) - 10;
        private IItem m_selectedItem;
        private bool m_enabled;
        private int m_cursorPosition;
        private List<ItemOptions> m_optionList;

        private DialogColorHelper m_dialogColorHelper;

        internal InventoryItemPainter()
        {
            m_enabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner)
        {
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                screen.DrawFrame(SelectedItemOffset, SelectedItemOffset, SelectedItemWidth, SelectedItemHeight, true);

                // Draw Header.
                screen.DrawHLine(SelectedItemOffset + 1, SelectedItemOffset + 2, SelectedItemWidth - 2);
                screen.PutChar(SelectedItemOffset, SelectedItemOffset + 2, SpecialCharacter.TEEE);
                screen.PutChar(SelectedItemOffset + SelectedItemWidth - 1, SelectedItemOffset + 2, SpecialCharacter.TEEW);
                screen.PrintLine(m_selectedItem.Name, SelectedItemOffset + (SelectedItemWidth / 2), SelectedItemOffset + 1, LineAlignment.Center);

                // Split in half for description.
                screen.DrawVLine(SelectedItemOffset + (SelectedItemWidth / 3), SelectedItemOffset + 2, SelectedItemHeight - 3);
                screen.PutChar(SelectedItemOffset + (SelectedItemWidth / 3), SelectedItemOffset + 2, SpecialCharacter.TEES);
                screen.PutChar(SelectedItemOffset + (SelectedItemWidth / 3), SelectedItemOffset + SelectedItemHeight - 1, SpecialCharacter.TEEN);

                screen.PrintLineRect(m_selectedItem.ItemDescription + m_selectedItem.FlavorDescription, SelectedItemOffset + ((SelectedItemWidth * 2) / 6) + 2, SelectedItemOffset + 4, ((SelectedItemWidth * 2) / 3) - 4, SelectedItemHeight - 6, LineAlignment.Left);

                m_dialogColorHelper.SaveColors(screen);
                
                // Print option list.
                for (int i = 0; i < m_optionList.Count; ++i)
                {
                    m_dialogColorHelper.SetColors(screen, i == m_cursorPosition, m_optionList[i].Enabled);
                    screen.PrintLine(m_optionList[i].Option, SelectedItemOffset + 2, SelectedItemOffset + 4 + (i * 2), Background.Set, LineAlignment.Left);
                }

                m_dialogColorHelper.ResetColors(screen);
            }   
        }

        public override void HandleRequest(string request, object data, object data2)
        {
            switch (request)
            {
                case "InventoryItemWindow":
                    m_selectedItem = (IItem)data;
                    m_optionList = (List<ItemOptions>)data2;
                    m_cursorPosition = 0;
                    m_enabled = true;
                    break;
                case "StopShowingInventoryItemWindow":
                    m_enabled = false;
                    break;
                case "InventoryItemPositionChanged":
                    Direction movementDirection = (Direction)data;
                    if (movementDirection == Direction.North)
                    {
                        if (m_cursorPosition > 0)
                            m_cursorPosition--;
                    }
                    if (movementDirection == Direction.South)
                    {
                        if (m_cursorPosition < (m_optionList.Count - 1))
                            m_cursorPosition++;
                    }
                    break;
                case "InventoryItemOptionSelected":
                    InventoryItemOptionSelected del = (InventoryItemOptionSelected)data;
                    if(m_optionList[m_cursorPosition].Enabled)
                        del(m_selectedItem, m_optionList[m_cursorPosition].Option);
                    break;
            }
        }
    }
}
