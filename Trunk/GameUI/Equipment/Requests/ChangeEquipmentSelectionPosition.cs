using System;

namespace Magecrawl.GameUI.Equipment.Requests
{
    public class ChangeEquipmentSelectionPosition : RequestBase
    {
        private Direction m_direction;

        public ChangeEquipmentSelectionPosition(Direction direction)
        {
            m_direction = direction;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            EquipmentPainter l = painter as EquipmentPainter;
            if (l != null)
                l.ChangeSelectionPosition(m_direction);
        }
    }
}
