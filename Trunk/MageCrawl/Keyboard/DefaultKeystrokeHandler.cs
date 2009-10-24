using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class DefaultKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public DefaultKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_keyMappings = null;
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return nothing.
         */

        private void North()
        {
            m_engine.MovePlayer(Direction.North);
            m_gameInstance.UpdatePainters();
        }

        private void South()
        {
            m_engine.MovePlayer(Direction.South);
            m_gameInstance.UpdatePainters();
        }

        private void East()
        {
            m_engine.MovePlayer(Direction.East);
            m_gameInstance.UpdatePainters();
        }

        private void West()
        {
            m_engine.MovePlayer(Direction.West);
            m_gameInstance.UpdatePainters();
        }

        private void Northeast()
        {
            m_engine.MovePlayer(Direction.Northeast);
            m_gameInstance.UpdatePainters();
        }

        private void Northwest()
        {
            m_engine.MovePlayer(Direction.Northwest);
            m_gameInstance.UpdatePainters();
        }

        private void Southeast()
        {
            m_engine.MovePlayer(Direction.Southeast);
            m_gameInstance.UpdatePainters();
        }

        private void Southwest()
        {
            m_engine.MovePlayer(Direction.Southwest);
            m_gameInstance.UpdatePainters();
        }

        private void Quit()
        {
            m_gameInstance.IsQuitting = true;
        }

        private void Operate()
        {
            m_gameInstance.SetHandlerName("Operate");
        }

        private void Save()
        {
            m_engine.Save();
            m_gameInstance.UpdatePainters();
        }

        private void Load()
        {
            try
            {
                m_engine.Load();
                m_gameInstance.UpdatePainters();
            }
            catch (System.IO.FileNotFoundException)
            {
                // TODO: Inform user somehow
                m_gameInstance.UpdatePainters();
            }
        }

        private void MoveableOnOff()
        {
            m_gameInstance.SendPaintersRequest("DebuggingMoveableOnOff", m_engine);
        }

        private void DebuggingFOVOnOff()
        {
            m_gameInstance.SendPaintersRequest("DebuggingFOVOnOff", m_engine);
        }

        private void Wait()
        {
            m_engine.PlayerWait();
            m_gameInstance.UpdatePainters();
        }

        private void Attack()
        {
             m_gameInstance.SetHandlerName("Attack");
        }

        private void ChangeWeapon()
        {
            m_engine.IterateThroughWeapons();
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

        private void Escape()
        {
        }

        #endregion
    }
}
