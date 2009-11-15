using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Equipment
{
    public delegate void EquipmentSelected(INamedItem item);

    internal sealed class EquipmentPainter : PainterBase
    {
        private const int EquipmentWindowOffset = 5;
        private const int EquipmentAdditionalHeightOffset = 12;
        private const int EquipmentWindowTopY = EquipmentAdditionalHeightOffset + EquipmentWindowOffset;
        private const int EquipmentItemWidth = UIHelper.ScreenWidth - 10;
        private const int EquipmentItemHeight = 18;

        private IPlayer m_player;
        private bool m_enabled;
        private int m_cursorPosition;
        private bool m_shouldNotResetCursorPosition;    // If set, the next time we show the equipment window, we don't reset the position.
        private List<string> m_equipmentListTypes = new List<string>() { "Weapon", "Headpiece", "Armor", "Gloves", "Boots" };

        private DialogColorHelper m_dialogColorHelper;

        internal EquipmentPainter()
        {
            m_enabled = false;
            m_dialogColorHelper = new DialogColorHelper();
            m_shouldNotResetCursorPosition = false;
        }

        public override void Dispose()
        {
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
        }

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                screen.DrawFrame(EquipmentWindowOffset, EquipmentWindowTopY, EquipmentItemWidth, EquipmentItemHeight, true, "Equipment");

                screen.DrawFrame(EquipmentWindowOffset + 1, EquipmentWindowTopY + EquipmentItemHeight - 6, EquipmentItemWidth - 2, 5, true);

                string weaponString = string.Format("Damage: {0}", m_player.CurrentWeapon.Damage.ToString());
                screen.PrintLine(weaponString, EquipmentWindowOffset + 3, EquipmentWindowTopY + EquipmentItemHeight - 4, LineAlignment.Left);

                List<INamedItem> equipmentList = CreateEquipmentListFromPlayer();

                m_dialogColorHelper.SaveColors(screen);
                for (int i = 0; i < equipmentList.Count; ++i)
                {
                    m_dialogColorHelper.SetColors(screen, i == m_cursorPosition, true);
                    screen.PrintLine(m_equipmentListTypes[i] + ":", EquipmentWindowOffset + 2, EquipmentWindowTopY + (2 * i) + 2, LineAlignment.Left);
                    if (equipmentList[i] != null)
                        screen.PrintLine(equipmentList[i].DisplayName, EquipmentWindowOffset + 14, EquipmentWindowTopY + (2 * i) + 2, LineAlignment.Left);
                    else
                        screen.PrintLine("None", EquipmentWindowOffset + 14, EquipmentWindowTopY + (2 * i) + 2, LineAlignment.Left);
                }
                m_dialogColorHelper.ResetColors(screen);
            }   
        }

        private List<INamedItem> CreateEquipmentListFromPlayer()
        {
            List<INamedItem> equipmentList = new List<INamedItem>();
            equipmentList.Add(m_player.CurrentWeapon);
            equipmentList.Add(null);
            equipmentList.Add(null);
            equipmentList.Add(null);
            equipmentList.Add(null);
            return equipmentList;
        }

        internal void Show(IPlayer player)
        {
            m_player = player;
            if (!m_shouldNotResetCursorPosition)
                m_cursorPosition = 0;
            m_shouldNotResetCursorPosition = false;
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
                if (m_cursorPosition < (m_equipmentListTypes.Count - 1))
                    m_cursorPosition++;
            }
        }

        internal void Select(EquipmentSelected onSelected)
        {
            onSelected(CreateEquipmentListFromPlayer()[m_cursorPosition]);
        }

        internal bool SaveSelectionPosition
        {
            get
            {
                return m_shouldNotResetCursorPosition;
            }
            set
            {
                m_shouldNotResetCursorPosition = value;
            }
        }
    }
}
