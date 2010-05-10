using System;
using System.Collections.Generic;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableTwoButtonDialog : RequestBase
    {
        private bool m_enable;
        private string m_text;

        public EnableTwoButtonDialog(bool enable, string textToDisplay)
        {
            m_enable = enable;
            m_text = textToDisplay;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            TwoButtonDialog h = painter as TwoButtonDialog;
            if (h != null)
            {
                h.Text = m_text;
                h.Enabled = m_enable;
            }
        }
    }
}
