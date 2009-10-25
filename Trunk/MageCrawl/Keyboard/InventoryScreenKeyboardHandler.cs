using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using System.Reflection;

namespace Magecrawl.Keyboard
{
    class InventoryScreenKeyboardHandler : BaseKeystrokeHandler
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
            else if(keystroke.Code == libtcodWrapper.KeyCode.TCODK_CHAR)
            {
                m_gameInstance.SendPaintersRequest("IntentoryItemSelectedByChar", new Magecrawl.GameUI.Map.InventoryItemSelected(ItemSelectedDelegate), keystroke.Character);
            }
        }
        
        private void ItemSelectedDelegate(IItem item)
        {
            m_gameInstance.UpdatePainters();
            m_gameInstance.TextBox.AddText("Selected: " + item.Name);
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest("IntentoryItemSelected", new Magecrawl.GameUI.Map.InventoryItemSelected(ItemSelectedDelegate));
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
