using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using Magecrawl.GameUI.Dialogs.Requests;
using Magecrawl.Keyboard;

namespace Magecrawl.GameUI.Dialogs
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "Help")]
    internal class HelpKeyboardHandler : BaseKeystrokeHandler
    {
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
