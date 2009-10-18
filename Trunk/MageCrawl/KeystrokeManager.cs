using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using libtcodWrapper;
using Magecrawl.GameEngine;

namespace MageCrawl
{
    internal enum KeystrokeResult
    {
        None,
        InOperate,
        PathableOn,
        Quit
    }

    internal struct NamedKey
    {
        internal KeyCode Code;
        internal char Character;

        public static NamedKey FromName(string name)
        {
            NamedKey key;
            try
            {
                key.Code = (KeyCode)Enum.Parse(typeof(KeyCode), name);
                key.Character = '\0';
            }
            catch (ArgumentException)
            {
                if (name.Length == 1)
                {
                    key.Code = KeyCode.TCODK_CHAR;
                    key.Character = name[0];
                }
                else
                {
                    throw new ArgumentException("Not a valid key name.", "name");
                }
            }
            return key;
        }
    }

    internal sealed class KeystrokeManager
    {
        private CoreGameEngine m_engine;
        private bool m_inOperate;
        private Dictionary<NamedKey, MethodInfo> m_keyMappings;

        internal KeystrokeManager(CoreGameEngine engine)
        {
            m_engine = engine;
            m_inOperate = false;

            // BCL: should we ensure that some or all actions have default mappings?
            m_keyMappings = new Dictionary<NamedKey, MethodInfo>();
        }

        public void LoadKeyMappings()
        {
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
                        if (!m_keyMappings.ContainsKey(namedKey))
                        {
                            m_keyMappings.Add(namedKey, action);
                        }
                        else
                        {
                            throw new InvalidOperationException(String.Format("That key has already been mapped."));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format("Could not find a mappable operation named {0}.", actionName));
                    }
                }
            }
            reader.Close();
        }

        public KeystrokeResult HandleKeyStroke()
        {
            KeyPress key = libtcodWrapper.Keyboard.CheckForKeypress(libtcodWrapper.KeyPressType.Pressed);
            MethodInfo action;
            NamedKey namedKey = new NamedKey() { Character = (char)key.Character, Code = key.KeyCode };

            if (m_keyMappings.TryGetValue(namedKey, out action))
            {
                return (KeystrokeResult)action.Invoke(this, null);
            }
            else
            {
                return KeystrokeResult.None;
            }
        }

        private KeystrokeResult OperateOrMove(Direction d)
        {
            if (m_inOperate)
                m_engine.Operate(d);
            else
                m_engine.MovePlayer(d);
            m_inOperate = false;
            return KeystrokeResult.None;
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return a 
         * KeyStrokeResult.
         */

        private KeystrokeResult North()
        {
            return OperateOrMove(Direction.North);
        }

        private KeystrokeResult South()
        {
            return OperateOrMove(Direction.South);
        }

        private KeystrokeResult East()
        {
            return OperateOrMove(Direction.East);
        }

        private KeystrokeResult West()
        {
            return OperateOrMove(Direction.West);
        }

        private KeystrokeResult Northeast()
        {
            return OperateOrMove(Direction.Northeast);
        }

        private KeystrokeResult Northwest()
        {
            return OperateOrMove(Direction.Northwest);
        }

        private KeystrokeResult Southeast()
        {
            return OperateOrMove(Direction.Southeast);
        }

        private KeystrokeResult Southwest()
        {
            return OperateOrMove(Direction.Southwest);
        }

        private KeystrokeResult Quit()
        {
            return KeystrokeResult.Quit;
        }

        private KeystrokeResult Operate()
        {
            m_inOperate = true;
            return KeystrokeResult.InOperate;
        }

        private KeystrokeResult Save()
        {
            m_engine.Save();
            return KeystrokeResult.None;
        }

        private KeystrokeResult Load()
        {
            try
            {
                m_engine.Load();
                return KeystrokeResult.None;
            }
            catch (System.IO.FileNotFoundException)
            {
                // TODO: Inform user somehow.
                return KeystrokeResult.None;
            }
        }

        private KeystrokeResult PathableOn()
        {
            return KeystrokeResult.PathableOn;
        }

        private KeystrokeResult Wait()
        {
            m_engine.PlayerWait();
            return KeystrokeResult.None;
        }

        #endregion
    }
}
