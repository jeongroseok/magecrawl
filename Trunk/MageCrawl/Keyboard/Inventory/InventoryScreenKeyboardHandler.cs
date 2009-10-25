using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.Keyboard.Inventory
{
    internal class InventoryScreenKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public InventoryScreenKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried()
        {
            m_gameInstance.SendPaintersRequest("DisableAllOverlays");
            m_gameInstance.SendPaintersRequest("ShowInventoryWindow");
            m_gameInstance.UpdatePainters();
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                action.Invoke(this, null);
            }
            else if (keystroke.Code == libtcodWrapper.KeyCode.TCODK_CHAR)
            {
                m_gameInstance.SendPaintersRequest("IntentoryItemSelectedByChar", new Magecrawl.GameUI.Inventory.InventoryItemSelected(ItemSelectedDelegate), keystroke.Character);
            }
        }
        
        private void ItemSelectedDelegate(IItem item)
        {
            m_gameInstance.SendPaintersRequest("StopShowingInventoryWindow");
            m_gameInstance.SetHandlerName("InventoryItem");
            
            List<ItemOptions> optionList = m_engine.GetOptionsForInventoryItem(item);
            m_gameInstance.SendPaintersRequest("InventoryItemWindow", item, optionList);
            m_gameInstance.UpdatePainters();
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest("IntentoryItemSelected", new Magecrawl.GameUI.Inventory.InventoryItemSelected(ItemSelectedDelegate));          
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest("StopShowingInventoryWindow");
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest("InventoryPositionChanged", direction);
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
