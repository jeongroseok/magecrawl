using System.ComponentModel.Composition;
using Magecrawl.GameUI.Inventory.Requests;
using Magecrawl.Interfaces;

namespace Magecrawl.Keyboard.Inventory
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "InventoryItem")]
    internal sealed class InventoryItemKeyboardHandler : InvokingKeystrokeHandler
    {
        private string m_handlerWhoCalledMe;

        public InventoryItemKeyboardHandler()
        {
            m_handlerWhoCalledMe = "";
        }

        public override void NowPrimaried(object request)
        {
            m_gameInstance.UpdatePainters();
            m_handlerWhoCalledMe = (string)request;
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new SelectInventoryItemOption(new Magecrawl.GameUI.Inventory.InventoryItemOptionSelected(InventoryItemOptionSelectedDelegate)));
        }

        private void InventoryItemOptionSelectedDelegate(IItem item, string optionName)
        {
            if (item == null)
            {
                Escape();
                return;
            }

            TargetingInfo targetInfo = m_engine.Targetting.GetTargettingTypeForInventoryItem(item, optionName);
            
            m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(false));

            HandleInvoke(item, targetInfo, x => m_engine.Actions.SelectedItemOption(item, optionName, x), NamedKey.Invalid);
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.SetHandlerName(m_handlerWhoCalledMe, true);   // Gets picked up in InventoryScreenKeyboardHandler::NowPrimaried
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest(new ChangeInventoryItemPosition(direction));
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
