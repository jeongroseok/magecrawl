using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Magecrawl.Keyboard
{
    internal abstract class BaseKeystrokeHandler : IKeystrokeHandler
    {
        protected Dictionary<NamedKey, MethodInfo> m_keyMappings = null;

        public void LoadKeyMappings(bool requireAllActions)
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

                    FindAndAddKeyHandler(requireAllActions, keyString, actionName);                   
                }
            }
            reader.Close();
        }

        private void FindAndAddKeyHandler(bool requireAllActions, string keyString, string actionName)
        {
            MethodInfo action = this.GetType().GetMethod(actionName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (action != null)
            {
                NamedKey namedKey = new NamedKey(keyString);
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
    }
}
