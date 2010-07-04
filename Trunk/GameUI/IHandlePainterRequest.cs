using System;
using System.Collections.Generic;
using System.Text;

namespace Magecrawl.GameUI
{
    internal interface IHandlePainterRequest
    {
        void HandleRequest(RequestBase request);
    }
}
