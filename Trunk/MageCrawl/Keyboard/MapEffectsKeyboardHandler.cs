using System.ComponentModel.Composition;
using Magecrawl.GameUI;

namespace Magecrawl.Keyboard
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "MapEffects")]
    internal class MapEffectsKeystrokeHandler : BaseKeystrokeHandler
    {
        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest((RequestBase)request);
            m_gameInstance.UpdatePainters();
        }
    }
}
