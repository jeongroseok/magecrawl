using System;
using System.Collections.Generic;
using System.Text;

namespace Magecrawl.GameUI
{
    internal interface IHandlePainterRequest
    {
    }

    public static class IHandlePainterRequestExtensions
    {
        internal static void HandleRequest(this IHandlePainterRequest handler, RequestBase request)
        {
            request.DoRequest(handler);
        }
    }
}
