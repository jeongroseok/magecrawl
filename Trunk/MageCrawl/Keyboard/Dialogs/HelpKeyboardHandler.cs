using System.Collections.Generic;
using System.Reflection;
using Magecrawl.Interfaces;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class HelpKeyboardHandler : BaseKeystrokeHandler
    {
        public HelpKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            Dictionary<NamedKey, MethodInfo> parentsKeymappings = (Dictionary<NamedKey, MethodInfo>)request;
            Dictionary<string, string> keyMappings = new Dictionary<string, string>();
            foreach (NamedKey k in parentsKeymappings.Keys)
            {
                string keyName = k.ToString();
                string methodName = parentsKeymappings[k].Name;
                if (!methodName.StartsWith("Debug"))
                    keyMappings[methodName] = keyName;
            }

            m_gameInstance.SendPaintersRequest(new EnableHelpDialog(true, keyMappings));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new EnableHelpDialog(false, null));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }
    }
}
