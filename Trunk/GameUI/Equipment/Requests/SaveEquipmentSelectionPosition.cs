using System;

namespace Magecrawl.GameUI.Equipment.Requests
{
    public class SaveEquipmentSelectionPosition : RequestBase
    {
        internal override void DoRequest(IHandlePainterRequest painter)
        {
            EquipmentPainter p = painter as EquipmentPainter;
            if (p != null)
                p.SaveSelectionPosition = true;
        }
    }
}
