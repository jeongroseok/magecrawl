using System;
using System.Collections.Generic;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableHelpDialog : RequestBase
    {
        private bool m_enable;
        private Dictionary<string, string> m_keyMappings;

        public EnableHelpDialog(bool enable, Dictionary<string, string> keyMappings)
        {
            m_enable = enable;
            m_keyMappings = keyMappings;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            HelpPainter h = painter as HelpPainter;
            if (h != null)
            {
                if (m_enable)
                    h.Enable(m_keyMappings);
                else
                    h.Disable();
            }
        }
    }
}
