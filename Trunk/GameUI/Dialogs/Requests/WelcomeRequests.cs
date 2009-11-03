using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameUI.Dialogs.Requests
{
    public class EnableWelcome : RequestBase
    {
        private bool m_enable;

        public EnableWelcome(bool enable)
        {
            m_enable = enable;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            WelcomePainter w = painter as WelcomePainter;
            if (w != null)
            {
                w.Enabled = m_enable;
            }
        }
    }
}
