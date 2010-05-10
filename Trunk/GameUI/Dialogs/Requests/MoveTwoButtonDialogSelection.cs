using System;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Dialogs;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class MoveTwoButtonDialogSelection : RequestBase
    {
        private bool m_left;

        public MoveTwoButtonDialogSelection(bool left)
        {
            m_left = left;
        }

        internal override void DoRequest (IHandlePainterRequest painter)
        {
            TwoButtonDialog painterAsTwoButtonDialog = painter as TwoButtonDialog;
            if(painterAsTwoButtonDialog != null)
            {
                painterAsTwoButtonDialog.MoveSelection(m_left);
            }
        }
    }
}
