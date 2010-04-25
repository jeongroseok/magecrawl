using System.Collections.Generic;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Inventory
{
    public delegate void InventoryItemOptionSelected(IItem item, string optionName);

    internal sealed class InventoryItemPainter : PainterBase
    {
        private const int SelectedItemOffsetX = 5;
        private const int SelectedItemOffsetY = 9;
        private const int SelectedItemWidth = UIHelper.ScreenWidth - (SelectedItemOffsetX * 2);
        private const int SelectedItemHeight = UIHelper.ScreenHeight - (SelectedItemOffsetY * 2) - 10;
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

        public override void DrawNewFrame(TCODConsole screen)
        {
            if (m_enabled)
            {
                screen.printFrame(SelectedItemOffsetX, SelectedItemOffsetY, SelectedItemWidth, SelectedItemHeight, true);

                // Draw Header.
                screen.hline(SelectedItemOffsetX + 1, SelectedItemOffsetY + 2, SelectedItemWidth - 2);
                screen.putChar(SelectedItemOffsetX, SelectedItemOffsetY + 2, (int)TCODSpecialCharacter.TeeEast);
                screen.putChar(SelectedItemOffsetX + SelectedItemWidth - 1, SelectedItemOffsetY + 2, (int)TCODSpecialCharacter.TeeWest);
                screen.printEx(SelectedItemOffsetX + (SelectedItemWidth / 2), SelectedItemOffsetY + 1, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_selectedItem.DisplayName);

                // Split in half for description.
                screen.vline(SelectedItemOffsetX + (SelectedItemWidth / 3), SelectedItemOffsetY + 2, SelectedItemHeight - 3);
                screen.putChar(SelectedItemOffsetX + (SelectedItemWidth / 3), SelectedItemOffsetY + 2, (int)TCODSpecialCharacter.TeeSouth);
                screen.putChar(SelectedItemOffsetX + (SelectedItemWidth / 3), SelectedItemOffsetY + SelectedItemHeight - 1, (int)TCODSpecialCharacter.TeeNorth);

                DrawItemInRightPane(screen);

                m_dialogColorHelper.SaveColors(screen);
                
                // Print option list.
                for (int i = 0; i < m_optionList.Count; ++i)
                {
                    m_dialogColorHelper.SetColors(screen, i == m_cursorPosition, m_optionList[i].Enabled);
                    screen.print(SelectedItemOffsetX + 2, SelectedItemOffsetY + 4 + (i * 2), m_optionList[i].Option);
                }

                m_dialogColorHelper.ResetColors(screen);
            }   
        }

        private void DrawItemInRightPane(TCODConsole screen)
        {
            string itemDescription = m_selectedItem.ItemDescription + "\n\n" + m_selectedItem.FlavorDescription;

            IWand asWand = m_selectedItem as IWand;
            if (asWand != null)
                itemDescription += "\n\n\n" + string.Format("Charges: {0} of {1}", asWand.Charges, asWand.MaxCharges);

            IArmor asArmor = m_selectedItem as IArmor;
            if (asArmor != null)
            {
                itemDescription += "\n";
                itemDescription += "\n\n" + string.Format("Defense: {0}", asArmor.Defense);
                itemDescription += "\n\n" + string.Format("Evade: {0}", asArmor.Evade);
                itemDescription += "\n\n" + string.Format("Weight: {0}", asArmor.Weight);
            }

            IWeapon asWeapon = m_selectedItem as IWeapon;
            if (asWeapon != null)
            {
                itemDescription += "\n";
                itemDescription += "\n\n" + string.Format("Damage: {0}", asWeapon.Damage);
            }

            screen.printRect(SelectedItemOffsetX + ((SelectedItemWidth * 2) / 6) + 2, SelectedItemOffsetY + 4, ((SelectedItemWidth * 2) / 3) - 4, SelectedItemHeight - 6, itemDescription);
        }

        internal void Show(IItem selectedItem, List<ItemOptions> optionList)
        {
            m_selectedItem = selectedItem;
            m_optionList = optionList;
            m_cursorPosition = 0;
            m_enabled = true;
        }

        internal void Hide()
        {
            m_enabled = false;
        }

        internal void ChangeSelectionPosition(Direction movementDirection)
        {
            if (movementDirection == Direction.North)
            {
                if (m_cursorPosition > 0)
                    m_cursorPosition--;
            }
            else if (movementDirection == Direction.South)
            {
                if (m_cursorPosition < (m_optionList.Count - 1))
                    m_cursorPosition++;
            }
        }

        internal void SelectOptionOnCurrent(InventoryItemOptionSelected onSelected)
        {
            // If there were no options and hit enter...
            if (m_optionList.Count == 0)
            {
                // Callback on null to cause windows to disappear then break.
                onSelected(null, null);
                return;
            }

            if (m_optionList[m_cursorPosition].Enabled)
                onSelected(m_selectedItem, m_optionList[m_cursorPosition].Option);
        }
    }
}
