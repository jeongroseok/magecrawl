using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreePainter : MapPainterBase
    {
        private bool m_enabled;
        
        internal SkillTreePainter()
        {
            m_enabled = false;
        }
        
        public override void DrawNewFrame (TCODConsole screen)
        {
            if (m_enabled)
            {
                screen.printFrame(10, 10, 40, 40, true, TCODBackgroundFlag.Set, "Skill Tree");
            }
        }
        
        public override void UpdateFromNewData (IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
        }
        
        internal bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }
    }
}
