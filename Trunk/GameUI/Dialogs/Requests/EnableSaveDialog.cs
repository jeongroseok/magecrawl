using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableSaveDialog : RequestBase
    {
        private bool m_enable;

        public EnableSaveDialog(bool enable)
        {
            m_enable = enable;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            SaveGamePainter s = painter as SaveGamePainter;
            if (s != null)
            {
                if (m_enable)
                {
                    s.Enable();
                }
                else
                {
                    s.Disable();
                }
            }
        }
    }
}
