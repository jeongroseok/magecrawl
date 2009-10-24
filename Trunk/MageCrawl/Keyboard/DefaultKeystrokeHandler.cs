using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal enum ChordKeystrokeStatus
    {
        None,
        Operate,
    }

    internal class DefaultKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private ChordKeystrokeStatus m_chordKeystroke;

        public DefaultKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_chordKeystroke = ChordKeystrokeStatus.None;
            m_keyMappings = null;
        }

        public override void NowPrimaried()
        {
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                action.Invoke(this, null);
            }
        }

        private void HandleDirection(Direction direction)
        {
            if (m_chordKeystroke == ChordKeystrokeStatus.Operate)
                m_engine.Operate(direction);
            else
                m_engine.MovePlayer(direction);
 
            m_chordKeystroke = ChordKeystrokeStatus.None;
            m_gameInstance.UpdatePainters();
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return nothing.
         */

        private void North()
        {
            HandleDirection(Direction.North);
        }

        private void South()
        {
            HandleDirection(Direction.South);
        }

        private void East()
        {
            HandleDirection(Direction.East);
        }

        private void West()
        {
            HandleDirection(Direction.West);
        }

        private void Northeast()
        {
            HandleDirection(Direction.Northeast);
        }

        private void Northwest()
        {
            HandleDirection(Direction.Northwest);
        }

        private void Southeast()
        {
            HandleDirection(Direction.Southeast);
        }

        private void Southwest()
        {
            HandleDirection(Direction.Southwest);
        }

        private void Quit()
        {
            m_gameInstance.IsQuitting = true;
        }

        private void Operate()
        {
            m_chordKeystroke = ChordKeystrokeStatus.Operate;
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
