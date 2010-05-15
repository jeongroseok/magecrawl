using System;

namespace Magecrawl.GameUI.Equipment.Requests
{
    public class EquipmentSelectedRequest : RequestBase
    {
        private EquipmentSelected m_onSelected;

        public EquipmentSelectedRequest(EquipmentSelected onSelected)
        {
            m_onSelected = onSelected;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            EquipmentPainter l = painter as EquipmentPainter;
            if (l != null)
                l.Select(m_onSelected);
        }
    }
}
