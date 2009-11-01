using System;
using System.Collections.Generic;
using System.Text;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.GameUI
{
    public abstract class RequestBase
    {
        public RequestBase()
        {
        }

        internal abstract void DoRequest(IHandlePainterRequest painter);
    }
}
