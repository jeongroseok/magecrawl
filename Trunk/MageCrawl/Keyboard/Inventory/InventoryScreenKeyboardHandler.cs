using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Inventory.Requests;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;

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

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            if (objOne != null && ((bool)objOne) == true)
                m_gameInstance.SendPaintersRequest(new SaveListSelectionPosition());
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, m_engine.Player.Items.OfType<INamedItem>().ToList(), "Inventory"));
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
                m_gameInstance.SendPaintersRequest(new ListSelectionItemSelectedByChar(keystroke.Character, new Magecrawl.GameUI.ListSelection.ListItemSelected(ItemSelectedDelegate)));
            }
        }

        private void ItemSelectedDelegate(INamedItem item)
        {
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false, null, null));
            m_gameInstance.SetHandlerName("InventoryItem");
            
            List<ItemOptions> optionList = m_engine.GetOptionsForInventoryItem((IItem)item);
            m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(true, (IItem)item, optionList));
            m_gameInstance.UpdatePainters();
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(new Magecrawl.GameUI.ListSelection.ListItemSelected(ItemSelectedDelegate)));          
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false, null, null));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest(new ChangeListSelectionPosition(direction));
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
