using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameUI.Equipment.Requests
{
    public class ShowEquipmentWindow : RequestBase
    {
        private bool m_show;
        private IPlayer m_player;

        public ShowEquipmentWindow(bool show)
        {
            m_show = show;
            m_player = null;

            if (m_show)
                throw new System.ArgumentException("ShowEquipmentWindow(bool show) must only be called if show is false");
        }

        public ShowEquipmentWindow(bool show, IPlayer player)
        {
            m_show = show;
            m_player = player;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            EquipmentPainter p = painter as EquipmentPainter;
            if (p != null)
            {
                if (m_show)
                    p.Show(m_player);
                else
                    p.Hide();
            }
        }
    }
}
