using System.Collections.Generic;
using System.Text;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    internal class CharacterInfo : PainterBase
    {
        public CharacterInfo()
        {
            m_player = null;
            m_currentLevel = -1;
            m_inDanger = false;
        }

        private const int StartingX = UIHelper.MapWidth;
        private const int InfoWidth = UIHelper.CharInfoWidth;
        private const int InfoHeight = UIHelper.CharInfoHeight;

        private const int ScreenCenter = StartingX + (InfoWidth / 2);

        private IPlayer m_player;
        private int m_currentLevel;
        private bool m_inDanger;

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_player = engine.Player;
            m_currentLevel = engine.CurrentLevel;
            m_inDanger = engine.DangerInLOS();
        }

        public override void DrawNewFrame(Console screen)
        {
            screen.DrawFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.PrintLine(m_player.Name, ScreenCenter, 1, LineAlignment.Center);

            string hpString = string.Format("HP: {0}/{1}", m_player.CurrentHP, m_player.MaxHP);
            screen.PrintLine(hpString, StartingX + 2, 2, LineAlignment.Left);
            string magicString = string.Format("Magic {0}/{1}", m_player.CurrentMP, m_player.MaxMP);
            screen.PrintLine(magicString, StartingX + 2, 3, LineAlignment.Left);

            screen.PrintLine("Status Effects:", StartingX + 2, 5, LineAlignment.Left);
            StringBuilder statusEffects = new StringBuilder();
            foreach (string s in m_player.StatusEffects)
            {
                statusEffects.Append(s + " ");
            }

            // TODO - What happens if this is more then 2 lines worth?
            screen.PrintLineRect(statusEffects.ToString(), StartingX + 2, 6, InfoWidth - 4, 2, LineAlignment.Left);

            if (Preferences.Instance.DebuggingMode)
            {
                string inDanger = m_inDanger ? "Danger" : "";
                screen.PrintLine(inDanger, 52, 55, LineAlignment.Left);

                string level = (m_currentLevel + 1).ToString();
                screen.PrintLine(level, 52, 56, LineAlignment.Left);

                string position = m_player.Position.ToString();
                screen.PrintLine(position, 52, 57, LineAlignment.Left);

                string fps = TCODSystem.FPS.ToString();
                screen.PrintLine(fps, 52, 58, LineAlignment.Left);
            }
        }
    }
}
