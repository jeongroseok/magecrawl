using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;

namespace MageCrawl
{
    internal enum ChordKeystrokeStatus
    {
        None,
        Operate,
        Attack
    }

    internal class DefaultKeystrokeHandler : IKeystrokeHandler
    {
        private IGameEngine m_engine;
        private ChordKeystrokeStatus m_chordKeystroke;
        private Dictionary<NamedKey, MethodInfo> m_keyMappings;

        public DefaultKeystrokeHandler(IGameEngine engine)
        {
            m_engine = engine;
            m_chordKeystroke = ChordKeystrokeStatus.None;
        }

        public KeystrokeResult HandleKeystroke(NamedKey keystroke)
        {
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                return (KeystrokeResult)action.Invoke(this, null);
            }
            else
            {
                return KeystrokeResult.None;
            }
        }

        public void LoadKeyMappings()
        {
            m_keyMappings = new Dictionary<NamedKey, MethodInfo>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader("KeyMappings.xml"), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "KeyMappings")
            {
                throw new InvalidOperationException("Bad key mappings file");
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "KeyMappings")
                {
                    break;
                }
                if (reader.LocalName == "KeyMapping")
                {
                    string key = reader.GetAttribute("Key");
                    string actionName = reader.GetAttribute("Action");
                    MethodInfo action = this.GetType().GetMethod(actionName, BindingFlags.Instance | BindingFlags.NonPublic);
                    if (action != null)
                    {
                        NamedKey namedKey = NamedKey.FromName(key);
                        m_keyMappings.Add(namedKey, action);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format("Could not find a mappable operation named {0}.", actionName));
                    }
                }
            }
            reader.Close();
        }

        private KeystrokeResult HandleDirection(Direction d)
        {
            if (m_chordKeystroke == ChordKeystrokeStatus.Operate)
                m_engine.Operate(d);
            else if (m_chordKeystroke == ChordKeystrokeStatus.Attack)
                m_engine.PlayerAttack(d);
            else
                m_engine.MovePlayer(d);
            m_chordKeystroke = ChordKeystrokeStatus.None;
            return KeystrokeResult.Action;
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return a 
         * KeyStrokeResult.
         */

        private KeystrokeResult North()
        {
            return HandleDirection(Direction.North);
        }

        private KeystrokeResult South()
        {
            return HandleDirection(Direction.South);
        }

        private KeystrokeResult East()
        {
            return HandleDirection(Direction.East);
        }

        private KeystrokeResult West()
        {
            return HandleDirection(Direction.West);
        }

        private KeystrokeResult Northeast()
        {
            return HandleDirection(Direction.Northeast);
        }

        private KeystrokeResult Northwest()
        {
            return HandleDirection(Direction.Northwest);
        }

        private KeystrokeResult Southeast()
        {
            return HandleDirection(Direction.Southeast);
        }

        private KeystrokeResult Southwest()
        {
            return HandleDirection(Direction.Southwest);
        }

        private KeystrokeResult Quit()
        {
            return KeystrokeResult.Quit;
        }

        private KeystrokeResult Operate()
        {
            m_chordKeystroke = ChordKeystrokeStatus.Operate;
            return KeystrokeResult.InOperate;
        }

        private KeystrokeResult Save()
        {
            m_engine.Save();
            return KeystrokeResult.Action;
        }

        private KeystrokeResult Load()
        {
            try
            {
                m_engine.Load();
                return KeystrokeResult.Action;
            }
            catch (System.IO.FileNotFoundException)
            {
                // TODO: Inform user somehow.
                return KeystrokeResult.Action;
            }
        }

        private KeystrokeResult MoveableOnOff()
        {
            return KeystrokeResult.DebuggingMoveableOnOff;
        }

        private KeystrokeResult Wait()
        {
            m_engine.PlayerWait();
            return KeystrokeResult.Action;
        }

        private KeystrokeResult Attack()
        {
            m_chordKeystroke = ChordKeystrokeStatus.Attack;
            return KeystrokeResult.InAttack;
        }

        private KeystrokeResult TextBoxPageUp()
        {
            return KeystrokeResult.TextBoxUp;
        }

        private KeystrokeResult TextBoxPageDown()
        {
            return KeystrokeResult.TextBoxDown;
        }

        private KeystrokeResult TextBoxClear()
        {
            return KeystrokeResult.TextBoxClear;
        }

        #endregion
    }
}
