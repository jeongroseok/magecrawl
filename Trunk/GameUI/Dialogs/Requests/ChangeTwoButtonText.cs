using System;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Dialogs;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class ChangeTwoButtonText : RequestBase
    {
        private string m_left;
        private string m_right;

        public ChangeTwoButtonText(string left, string right)
        {
            m_left = left;
            m_right = right;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            TwoButtonDialog twoButtonDialog = painter as TwoButtonDialog;
            if (twoButtonDialog != null)
            {
                twoButtonDialog.ChangeButtonText(m_left, m_right);
            }
        }
    }
}
