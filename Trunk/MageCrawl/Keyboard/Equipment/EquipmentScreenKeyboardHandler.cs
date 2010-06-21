using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.GameUI.Equipment;
using Magecrawl.GameUI.Equipment.Requests;
using Magecrawl.GameUI.Inventory.Requests;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.Keyboard.Inventory
{
    internal class EquipmentScreenKeyboardHandler : BaseKeystrokeHandler
    {
        public EquipmentScreenKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            if (request != null && ((bool)request) == true)
                m_gameInstance.SendPaintersRequest(new SaveEquipmentSelectionPosition());
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            m_gameInstance.SendPaintersRequest(new ShowEquipmentWindow(true, m_engine.Player));
            m_gameInstance.UpdatePainters();
        }

        private void EquipmentSelectedDelegate(INamedItem item)
        {
            // Don't try to pop out information on slots without equipment or "your fists"
            if (item != null && item.DisplayName != "Melee")
            {
                m_gameInstance.SendPaintersRequest(new ShowEquipmentWindow(false));
                m_gameInstance.SetHandlerName("InventoryItem", "Equipment");

                List<ItemOptions> optionList = m_engine.GetOptionsForEquipmentItem((IItem)item);
                m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(true, (IItem)item, optionList));
            }
            m_gameInstance.UpdatePainters();
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new EquipmentSelectedRequest(new EquipmentSelected(EquipmentSelectedDelegate)));          
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowEquipmentWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest(new ChangeEquipmentSelectionPosition(direction));
            m_gameInstance.UpdatePainters();
        }

        private void North()
        {
            HandleDirection(Direction.North);
        }

        private void South()
        {
            HandleDirection(Direction.South);
        }
    }
}
