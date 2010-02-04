using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Magecrawl.GameUI;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal abstract class BaseKeystrokeHandler : IKeystrokeHandler
    {
        public static string ErrorsParsingKeymapFiles = String.Empty;
        protected Dictionary<NamedKey, MethodInfo> m_keyMappings = null;
        protected NamedKey m_enterKey;

        public void LoadKeyMappings(bool requireAllActions)
        {
            m_keyMappings = new Dictionary<NamedKey, MethodInfo>();
            ParseKeymappingFile(requireAllActions, "KeyMappings.xml");
            ParseKeymappingFile(requireAllActions, GetArrowMappingConfigPath());
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
                if ((string)Preferences.Instance["CustomKeymappingFilename"] != String.Empty)
                {
                    string customFileName = (string)Preferences.Instance["CustomKeymappingFilename"];
                    if (File.Exists(customFileName))
                    {
                        return customFileName;
                    }
                    else
                    {
                        AddErrorIfNotReported("Error in keymapping - unable to find \"" + customFileName + "\" for keymapping.\nUsing \"Arrows\" for default.\n");
                        return Path.Combine("Resources", "ArrowKeysMapping.xml"); ;
                    }
                }
                else
                {
                    AddErrorIfNotReported("Error in keymapping - CustomKeymappingFilename is not set.\nUsing \"Arrows\" for default.\n");
                    return Path.Combine("Resources", "ArrowKeysMapping.xml"); ;
                }

            }
            throw new InvalidOperationException("Unable to locate keymapping");
        }

        private void AddErrorIfNotReported(string s)
        {
            if (!ErrorsParsingKeymapFiles.Contains(s))
                ErrorsParsingKeymapFiles += s;
        }

        private void ParseKeymappingFile(bool requireAllActions, string fileToParse)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(fileToParse), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "KeyMappings")
            {
                throw new System.InvalidOperationException("Bad key mappings file");
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
                    string keyString = reader.GetAttribute("Key");
                    string actionName = reader.GetAttribute("Action");

                    if (isDirectionAction(actionName))
                        FindAndAddKeyHandler(requireAllActions, keyString + "Control", "Run" + actionName);

                    if (actionName == "Select")
                        m_enterKey = new NamedKey(keyString);

                    FindAndAddKeyHandler(requireAllActions, keyString, actionName);
                }
            }
            reader.Close();
        }

        private void FindAndAddKeyHandler(bool requireAllActions, string keyString, string actionName)
        {
            MethodInfo action = this.GetType().GetMethod(actionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            if (action != null)
            {
                NamedKey namedKey = new NamedKey(keyString);
                if (m_keyMappings.ContainsKey(namedKey))
                {
                    AddErrorIfNotReported("Error in keymapping - Trying to assign multiple keys to " + namedKey.ToString() + ".\n");
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

        public virtual void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
        }

        protected void AlternateSelect()
        {
            HandleKeystroke(m_enterKey);
        }
    }
}
