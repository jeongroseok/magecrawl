using System.Linq;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class DefaultKeystrokeHandler : BaseKeystrokeHandler
    {
        private PlayerActions m_playerActions;

        public DefaultKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_playerActions = new PlayerActions(engine, instance);
        }
        
        private NamedKey GetNamedKeyForMethodInfo(MethodInfo info)
        {
            return m_keyMappings.Keys.Where(k => m_keyMappings[k] == info).Single();
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return nothing.
         */

        // If you add new non-debug commands, remember to update HelpPainter.cs
        private void North()
        {
            m_playerActions.Move(Direction.North);            
        }

        private void South()
        {
            m_playerActions.Move(Direction.South);
        }

        private void East()
        {
            m_playerActions.Move(Direction.East);
        }

        private void West()
        {
            m_playerActions.Move(Direction.West);
        }

        private void Northeast()
        {
            m_playerActions.Move(Direction.Northeast);
        }

        private void Northwest()
        {
            m_playerActions.Move(Direction.Northwest);
        }

        private void Southeast()
        {
            m_playerActions.Move(Direction.Southeast);
        }

        private void Southwest()
        {
            m_playerActions.Move(Direction.Southwest);
        }

        private void RunNorth()
        {
            m_playerActions.Run(Direction.North);
        }

        private void RunSouth()
        {
            m_playerActions.Run(Direction.South);
        }

        private void RunEast()
        {
            m_playerActions.Run(Direction.East);
        }

        private void RunWest()
        {
            m_playerActions.Run(Direction.West);
        }

        private void RunNortheast()
        {
            m_playerActions.Run(Direction.Northeast);
        }

        private void RunNorthwest()
        {
            m_playerActions.Run(Direction.Northwest);
        }

        private void RunSoutheast()
        {
            m_playerActions.Run(Direction.Southeast);
        }

        private void RunSouthwest()
        {
            m_playerActions.Run(Direction.Southwest);
        }

        private void Quit()
        {
            m_gameInstance.SetHandlerName("QuitGame", QuitReason.quitAction);
        }

        private void Operate()
        {
            NamedKey operateKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_playerActions.Operate(operateKey);
        }

        private void GetItem()
        {
            m_playerActions.GetItem();
        }

        private void Save()
        {
            m_gameInstance.SetHandlerName("SaveGame");
        }

        private void DebugMode()
        {
            if (Preferences.Instance.DebuggingMode)
            {
                m_gameInstance.SetHandlerName("DebugMode");
                m_gameInstance.UpdatePainters();
            }
        }

        private void Wait()
        {
            m_playerActions.Wait();
        }

        private void Attack()
        {
            NamedKey attackKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_playerActions.Attack(attackKey);            
        }

        private void ViewMode()
        {
            m_gameInstance.SetHandlerName("Viewmode");
        }

        private void Inventory()
        {
            m_gameInstance.SetHandlerName("Inventory");
        }

        private void Equipment()
        {
            m_gameInstance.SetHandlerName("Equipment");
        }

        private void SwapWeapon()
        {
            m_playerActions.SwapWeapon();
        }

        private void TextBoxPageUp()
        {
            m_gameInstance.TextBox.TextBoxScrollUp();
        }

        private void TextBoxPageDown()
        {
            m_gameInstance.TextBox.TextBoxScrollDown();
        }

        private void TextBoxClear()
        {
            m_gameInstance.TextBox.Clear();
        }

        private void CastSpell()
        {
            NamedKey castKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_gameInstance.SetHandlerName("SpellList", castKey);
        }
        
        private void Escape()
        {
        }

        private void Select()
        {
        }

        private void Help()
        {
            m_gameInstance.SetHandlerName("Help", m_keyMappings);
        }

        private void DownStairs()
        {
            m_playerActions.DownStairs();
        }

        private void UpStairs()
        {
            m_playerActions.UpStairs();
        }

        private void MoveToLocation()
        {
            NamedKey movementKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_playerActions.MoveToLocation(movementKey);
        }

        // If you add new non-debug commands, remember to update HelpPainter.cs
        #endregion
    }
}
