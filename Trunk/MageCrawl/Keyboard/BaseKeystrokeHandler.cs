using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal abstract class BaseKeystrokeHandler : IKeystrokeHandler
    {
        protected IGameEngine m_engine;
        protected GameInstance m_gameInstance;

        private static StreamReader m_keyMappingsStream;
        private static StreamReader m_arrowMappingsStream;

        public static string ErrorsParsingKeymapFiles = "";
        protected Dictionary<NamedKey, MethodInfo> m_keyMappings;
        protected Dictionary<string, NamedKey> m_actionKeyMapping;

        public void Init(IGameEngine engine, GameInstance gameInstance, bool requireAllActions)
        {
            m_engine = engine;
            m_gameInstance = gameInstance;

            m_keyMappings = new Dictionary<NamedKey, MethodInfo>();
            m_actionKeyMapping = new Dictionary<string, NamedKey>();

            // So we don't hit disk n times, where n is the number of Keyboard handlers, cache the file to read over and over.
            if (m_keyMappingsStream == null)
            {
                m_keyMappingsStream = new StreamReader("KeyMappings.xml");
                m_arrowMappingsStream = new StreamReader(GetArrowMappingConfigPath());                
            }
            else
            {
                m_keyMappingsStream.BaseStream.Seek(0, SeekOrigin.Begin);
                m_arrowMappingsStream.BaseStream.Seek(0, SeekOrigin.Begin);
            }

            // We depend on this order so specific actions "like rest" can be overwritten by arrow mapping files.
            ParseKeymappingFile(requireAllActions, m_keyMappingsStream);
            ParseKeymappingFile(requireAllActions, m_arrowMappingsStream);
        }

        private string GetArrowMappingConfigPath()
        {
            string mappingName = (string)Preferences.Instance["Keymapping"];
            if (mappingName == "Arrows")
            {
                return Path.Combine("Resources", "ArrowKeysMapping.xml");
            }
            else if (mappingName == "Keypad")
            {
                return Path.Combine("Resources", "KeypadKeysMapping.xml");
            }
            else if (mappingName == "VIM")
            {
                return Path.Combine("Resources", "VimKeysMapping.xml");
            }
            else if (mappingName == "Custom")
            {
                if ((string)Preferences.Instance["CustomKeymappingFilename"] != "")
                {
                    string customFileName = (string)Preferences.Instance["CustomKeymappingFilename"];
                    if (File.Exists(customFileName))
                    {
                        return customFileName;
                    }
                    else
                    {
                        AddErrorIfNotReported("Error in keymapping - unable to find \"" + customFileName + "\" for keymapping.\nUsing \"Arrows\" for default.\n");
                        return Path.Combine("Resources", "ArrowKeysMapping.xml");
                    }
                }
                else
                {
                    AddErrorIfNotReported("Error in keymapping - CustomKeymappingFilename is not set.\nUsing \"Arrows\" for default.\n");
                    return Path.Combine("Resources", "ArrowKeysMapping.xml");
                }
            }
            throw new InvalidOperationException("Unable to locate keymapping");
        }

        private void AddErrorIfNotReported(string s)
        {
            if (!ErrorsParsingKeymapFiles.Contains(s))
                ErrorsParsingKeymapFiles += s;
        }

        private void ParseKeymappingFile(bool requireAllActions, StreamReader stream)
        {
            XMLResourceReaderBase.ParseFile(stream, ReadFileCallback, requireAllActions);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            bool requireAllActions = (bool)data;
            if (reader.LocalName != "KeyMappings")
                throw new System.InvalidOperationException("Bad key mappings file");

            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "KeyMappings")
                {
                    break;
                }
                if (reader.LocalName == "KeyMapping")
                {
                    string keyString = reader.GetAttribute("Key");
                    string actionName = reader.GetAttribute("Action");

                    if (isDirectionAction(actionName))
                    {
                        string runSuffix = (bool)Preferences.Instance["UseAltInsteadOfCtrlForRunning"] ? "Alt" : "Control";
                        FindAndAddKeyHandler(requireAllActions, keyString + runSuffix, "Run" + actionName);
                        m_actionKeyMapping["Run" + actionName] = new NamedKey(keyString + runSuffix);
                    }
                    if (actionName == "ChangeTabs")
                    {
                        FindAndAddKeyHandler(requireAllActions, keyString + "Shift", "Shift" + actionName);
                        m_actionKeyMapping["Shift" + actionName] = new NamedKey(keyString + "Shift");
                    }

                    m_actionKeyMapping[actionName] = new NamedKey(keyString);
                    FindAndAddKeyHandler(requireAllActions, keyString, actionName);
                }
            }
        }

        private void FindAndAddKeyHandler(bool requireAllActions, string keyString, string actionName)
        {
            MethodInfo action = this.GetType().GetMethod(actionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            if (action != null)
            {
                NamedKey namedKey = new NamedKey(keyString);
                if (m_keyMappings.ContainsKey(namedKey))
                {
                    AddErrorIfNotReported("Error in keymapping - Trying to assign multiple keys to '" + namedKey.ToString() + "'.\n");
                    m_keyMappings.Remove(namedKey);
                }
                m_keyMappings.Add(namedKey, action);
            }
            else if (requireAllActions)
            {
                throw new InvalidOperationException(string.Format("Could not find a mappable operation named {0}.", actionName));
            }
        }

        private bool isDirectionAction(string action)
        {
            switch (action)
            {
                case "North":
                case "South":
                case "West":
                case "East":
                case "Northwest":
                case "Southwest":
                case "Northeast":
                case "Southeast":
                case "AlternateNorth":
                case "AlternateSouth":
                case "AlternateWest":
                case "AlternateEast":
                    return true;
                default:
                    return false;
            }
        }

        public virtual void HandleKeystroke(NamedKey keystroke)
        {
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                action.Invoke(this, null);
            }
        }

        public virtual void NowPrimaried(object request)
        {
        }

        protected virtual void ChangeTabs()
        {
        }

        protected virtual void ShiftChangeTabs()
        {
        }

        protected void AlternateNorth()
        {
            HandleKeystroke(m_actionKeyMapping["North"]);
        }

        protected void AlternateSouth()
        {
            HandleKeystroke(m_actionKeyMapping["South"]);
        }

        protected void AlternateWest()
        {
            HandleKeystroke(m_actionKeyMapping["West"]);
        }

        protected void AlternateEast()
        {
            HandleKeystroke(m_actionKeyMapping["East"]);
        }

        protected void RunAlternateNorth()
        {
            HandleKeystroke(m_actionKeyMapping["RunNorth"]);
        }

        protected void RunAlternateSouth()
        {
            HandleKeystroke(m_actionKeyMapping["RunSouth"]);
        }

        protected void RunAlternateWest()
        {
            HandleKeystroke(m_actionKeyMapping["RunWest"]);
        }

        protected void RunAlternateEast()
        {
            HandleKeystroke(m_actionKeyMapping["RunEast"]);
        }

        protected void AlternateSelect()
        {
            HandleKeystroke(m_actionKeyMapping["Select"]);
        }
    }
}
