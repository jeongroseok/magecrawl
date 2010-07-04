using System.ComponentModel.Composition;
using System.Linq;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Interfaces;

namespace Magecrawl.Keyboard.Effects
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "ShowEffects")]
    internal class ShowEffectsHandler : BaseKeystrokeHandler
    {
        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, m_engine.Player.StatusEffects.OfType<INamedItem>().ToList(),
                                                                           false, "Dismiss Effect", i => ((IStatusEffect)i).IsPositiveEffect));
            m_gameInstance.UpdatePainters();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void North()
        {
            m_gameInstance.SendPaintersRequest(new ChangeListSelectionPosition(Direction.North));
        }

        private void South()
        {
            m_gameInstance.SendPaintersRequest(new ChangeListSelectionPosition(Direction.South));
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(OnSelect));
        }

        private void OnSelect(INamedItem item)
        {
            m_engine.Actions.DismissEffect(item.DisplayName);
            Escape();
        }
    }
}
