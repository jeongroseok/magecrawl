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

        private int HealthBarLength(ICharacter character, bool fuzzy)
        {
            if (fuzzy)
            {
                int value = (int)Math.Floor(((double)character.CurrentHP / (double)character.MaxHP) * 20.0);
                if (value >= 19)
                    return 20;
                else if (value >= 14)
                    return 15;
                else if (value >= 7)
                    return 10;
                else
                    return 5;
            }
            else
                return (int)Math.Floor(((double)character.CurrentHP / (double)character.MaxHP) * 20.0);
        }

        private int ManaBarLength(IPlayer character)
        {
            return (int)Math.Floor(((double)character.CurrentMP / (double)character.MaxMP) * 20.0);
        }

        private Color EnemyHealthBarColor(ICharacter character)
        {
            double percentage = ((double)character.CurrentHP / (double)character.MaxHP) * 100;
            if (percentage > 95)
                return ColorPresets.DarkBlue / 2;
            else if (percentage > 70)
                return ColorPresets.DarkGreen / 2;
            else if (percentage > 35)
                return ColorPresets.DarkOrange / 2;
            else
                return ColorPresets.DarkRed / 2;
        }

        private Color PlayerHealthBarColor(ICharacter character)
        {
            double percentage = ((double)character.CurrentHP / (double)character.MaxHP) * 100;
            if (percentage > 95)
                return ColorPresets.Red / 2;
            else if (percentage > 70)
                return ColorPresets.Red / 2.5;
            else if (percentage > 35)
                return ColorPresets.Red / 3;
            else
                return ColorPresets.DarkRed / 2;
        }

        private Color ManaBarColor(IPlayer character)
        {
            double percentage = ((double)character.CurrentMP / (double)character.MaxMP) * 100;
            if (percentage > 95)
                return ColorPresets.Blue / 1.5;
            else if (percentage > 70)
                return ColorPresets.Blue / 2;
            else if (percentage > 35)
                return ColorPresets.Blue / 3;
            else
                return ColorPresets.Blue / 4;
        }

        public override void DrawNewFrame(libtcodWrapper.Console screen)
        {
            screen.DrawFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.PrintLine(m_player.Name, ScreenCenter, 1, LineAlignment.Center);

            string hpString = string.Format("HP: {0}/{1}", m_player.CurrentHP, m_player.MaxHP);
            screen.PrintLine(hpString, StartingX + 12, 2, LineAlignment.Center);
            for (int j = 0; j < HealthBarLength(m_player, false); ++j)
                screen.SetCharBackground(StartingX + 2 + j, 2, PlayerHealthBarColor(m_player));


            string magicString = string.Format("Magic {0}/{1}", m_player.CurrentMP, m_player.MaxMP);
            screen.PrintLine(magicString, StartingX + 12, 3, LineAlignment.Center);
            for (int j = 0; j < ManaBarLength(m_player); ++j)
                screen.SetCharBackground(StartingX + 2 + j, 3, ManaBarColor(m_player));

            int nextAvailablePosition = 6;

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
                    for (int j = 0; j < HealthBarLength(currentMonster, true); ++j)
                        screen.SetCharBackground(StartingX + 2 + j, nextAvailablePosition + 1 + i, EnemyHealthBarColor(currentMonster));
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
