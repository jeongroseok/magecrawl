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
