using System;
using System.Collections.Generic;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class DisableTwoButtonDialog : RequestBase
    {
        public delegate void GetDialogValue(bool leftButton);

        private GetDialogValue m_delegate;

        public DisableTwoButtonDialog(GetDialogValue returnValueDelegate)
        {
             m_delegate = returnValueDelegate;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            TwoButtonDialog h = painter as TwoButtonDialog;
            if (h != null)
            {
                m_delegate(h.LeftButtonSelected);
                h.Enabled = false;
            }
        }
    }
}
