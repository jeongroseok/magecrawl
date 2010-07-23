using System.Collections.Generic;
using libtcod;
using Magecrawl.Interfaces;
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
        private IPlayer m_player;

        private DialogColorHelper m_dialogColorHelper;

        internal InventoryItemPainter()
        {
            m_enabled = false;
            m_dialogColorHelper = new DialogColorHelper();
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_player = engine.Player;
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
            int x = SelectedItemOffsetX + ((SelectedItemWidth * 2) / 6) + 2;
            int y = SelectedItemOffsetY + 4;
            int w = ((SelectedItemWidth * 2) / 3) - 4;
            int h = SelectedItemHeight - 6;

            string itemDescription = m_selectedItem.ItemDescription + "\n\n" + m_selectedItem.FlavorDescription;
            y += screen.printRect(x, y, w, h, itemDescription);
            y += 2;

            IConsumable asConsumable = m_selectedItem as IConsumable;
            if (asConsumable != null && asConsumable.MaxCharges > 1)
            {
                screen.print(x, y, string.Format("Charges: {0} of {1}", asConsumable.Charges, asConsumable.MaxCharges));
                y++;
            }

            IArmor asArmor = m_selectedItem as IArmor;
            if (asArmor != null)
            {
                screen.print(x, y, "Stamina Bonus: " + asArmor.StaminaBonus);
                y += 2;
                screen.print(x, y, "Evade: " + asArmor.Evade);
                y += 2;

                IList<EquipArmorReasons> armorReasons = m_player.CanNotEquipArmorReasons(asArmor);
                if (armorReasons.Contains(EquipArmorReasons.Weight))
                {
                    m_dialogColorHelper.SaveColors(screen);
                    screen.setForegroundColor(TCODColor.red);
                    screen.print(x, y, "Weight: " + asArmor.Weight);
                    y += 2;
                    m_dialogColorHelper.ResetColors(screen);
                }
                else
                {
                    screen.print(x, y, "Weight: " + asArmor.Weight);
                    y += 2;
                }

                if (armorReasons.Contains(EquipArmorReasons.RobesPreventBoots) || armorReasons.Contains(EquipArmorReasons.BootsPreventRobes))
                {
                    m_dialogColorHelper.SaveColors(screen);
                    screen.setForegroundColor(TCODColor.red);
                    string outputString = armorReasons.Contains(EquipArmorReasons.RobesPreventBoots) ? "Robes prevent boots from being worn." : "Boots prevent robes from being worn.";
                    screen.print(x, y, outputString);
                    y += 2;
                    m_dialogColorHelper.ResetColors(screen);
                }
            }

            IWeapon asWeapon = m_selectedItem as IWeapon;
            if (asWeapon != null)
            {
                screen.print(x, y, "Damage: " + asWeapon.Damage);                
            }
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
