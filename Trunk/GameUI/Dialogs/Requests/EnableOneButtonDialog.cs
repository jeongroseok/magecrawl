using System;
using System.Collections.Generic;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableOneButtonDialog : RequestBase
    {
        private bool m_enable;
        private string m_text;

        public EnableOneButtonDialog(bool enable, string textToDisplay)
        {
            m_enable = enable;
            m_text = textToDisplay;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            OneButtonDialog h = painter as OneButtonDialog;
            if (h != null)
            {
                if (m_enable)
                    h.Enable(m_text);
                else
                    h.Disable();
            }
        }
    }
}
