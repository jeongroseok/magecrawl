using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
        private List<ICharacter> m_monstersNearby;

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_player = engine.Player;
            m_currentLevel = engine.CurrentLevel;
            m_inDanger = engine.DangerInLOS();
            m_monstersNearby = engine.MonstersInLOS().OrderBy(x => PointDirectionUtils.LatticeDistance(x.Position, m_player.Position)).ToList();
        }

        private int HealthBarLength(ICharacter character)
        {
            return (int)Math.Floor(((double)character.CurrentHP / (double)character.MaxHP) * 20.0);
        }

        private Color HealthBarColor(ICharacter character)
        {
            double percentage = ((double)character.CurrentHP / (double)character.MaxHP) * 100;
            if (percentage > 95)
                return ColorPresets.Blue;
            else if (percentage > 70)
                return ColorPresets.Green;
            else if (percentage > 35)
                return ColorPresets.DarkOrange;
            else
                return ColorPresets.Red;
        }

        public override void DrawNewFrame(libtcodWrapper.Console screen)
        {
            screen.DrawFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.PrintLine(m_player.Name, ScreenCenter, 1, LineAlignment.Center);

            string hpString = string.Format("HP: {0}/{1}", m_player.CurrentHP, m_player.MaxHP);
            screen.PrintLine(hpString, StartingX + 2, 2, LineAlignment.Left);
            string magicString = string.Format("Magic {0}/{1}", m_player.CurrentMP, m_player.MaxMP);
            screen.PrintLine(magicString, StartingX + 2, 3, LineAlignment.Left);

            int nextAvailablePosition = 5;

            if (m_player.StatusEffects.Count > 0)
            {
                screen.PrintLine("Status Effects:", StartingX + 2, nextAvailablePosition, LineAlignment.Left);
                StringBuilder statusEffects = new StringBuilder();
                foreach (string s in m_player.StatusEffects)
                {
                    statusEffects.Append(s + " ");
                }

                // TODO - What happens if this is more then 2 lines worth?
                int statusEfectLength = screen.PrintLineRect(statusEffects.ToString(), StartingX + 2, nextAvailablePosition+1, InfoWidth - 4, 2, LineAlignment.Left);

                nextAvailablePosition += 2 + statusEfectLength;
            }

            if (m_monstersNearby.Count > 0)
            {
                string countAmount = m_monstersNearby.Count < 8 ? m_monstersNearby.Count.ToString() : "8+";
                screen.PrintLine(string.Format("Nearby Enemies ({0}):", countAmount), StartingX + 2, nextAvailablePosition, LineAlignment.Left);
                for (int i = 0; i < m_monstersNearby.Count; ++i)
                {
                    ICharacter currentMonster = m_monstersNearby[i];
                    screen.PrintLine(currentMonster.Name, StartingX + 12, nextAvailablePosition + 1 + i, LineAlignment.Center);
                    for (int j = 0; j < HealthBarLength(currentMonster); ++j)
                        screen.SetCharBackground(StartingX + 2 + j, nextAvailablePosition + 1 + i, HealthBarColor(currentMonster));
                }
                nextAvailablePosition += 2;
            }

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
