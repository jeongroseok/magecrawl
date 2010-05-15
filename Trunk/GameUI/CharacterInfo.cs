using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
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
        }

        private const int StartingX = UIHelper.MapWidth;
        private const int InfoWidth = UIHelper.CharInfoWidth;
        private const int InfoHeight = UIHelper.CharInfoHeight;

        private const int ScreenCenter = StartingX + (InfoWidth / 2);

        private IPlayer m_player;
        private int m_currentLevel;
        private List<ICharacter> m_monstersNearby;
        private int m_turnCount;

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_player = engine.Player;
            m_currentLevel = engine.CurrentLevel;
            m_monstersNearby = engine.MonstersInPlayerLOS().OrderBy(x => PointDirectionUtils.LatticeDistance(x.Position, m_player.Position)).ToList();
            m_turnCount = engine.TurnCount;
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

        private TCODColor EnemyHealthBarColor(ICharacter character)
        {
            double percentage = ((double)character.CurrentHP / (double)character.MaxHP) * 100;
            if (percentage > 95)
                return TCODColor.darkBlue.Divide(2);
            else if (percentage > 70)
                return TCODColor.darkGreen.Divide(2);
            else if (percentage > 35)
                return TCODColor.darkOrange.Divide(2);
            else
                return TCODColor.darkRed.Divide(2);
        }

        private TCODColor PlayerHealthBarColor(ICharacter character)
        {
            double percentage = ((double)character.CurrentHP / (double)character.MaxHP) * 100;
            if (percentage > 95)
                return TCODColor.red.Divide(2);
            else if (percentage > 70)
                return TCODColor.red.Divide(2.5);
            else if (percentage > 35)
                return TCODColor.red.Divide(3);
            else
                return TCODColor.darkRed.Divide(2);
        }

        private TCODColor ManaBarColor(IPlayer character)
        {
            double percentage = ((double)character.CurrentMP / (double)character.MaxMP) * 100;
            if (percentage > 95)
                return TCODColor.blue.Divide(1.5);
            else if (percentage > 70)
                return TCODColor.blue.Divide(2);
            else if (percentage > 35)
                return TCODColor.blue.Divide(3);
            else
                return TCODColor.blue.Divide(4);
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            screen.printFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.printEx(ScreenCenter, 1, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_player.Name);

            string hpString = string.Format("HP: {0}/{1}", m_player.CurrentHP, m_player.MaxHP);
            screen.printEx(StartingX + 12, 2, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, hpString);
            for (int j = 0; j < HealthBarLength(m_player, false); ++j)
                screen.setCharBackground(StartingX + 2 + j, 2, PlayerHealthBarColor(m_player));

            string magicString = string.Format("Magic {0}/{1}", m_player.CurrentMP, m_player.MaxMP);
            screen.printEx(StartingX + 12, 3, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, magicString);
            for (int j = 0; j < ManaBarLength(m_player); ++j)
                screen.setCharBackground(StartingX + 2 + j, 3, ManaBarColor(m_player));

            string needsLoadedString = m_player.CurrentWeapon.IsLoaded ? "" : "(empty)";
            screen.print(StartingX + 2, 6, string.Format("Weapon: {0}{1}", m_player.CurrentWeapon.DisplayName, needsLoadedString));
            
            int nextAvailablePosition = 8;

            if (m_player.StatusEffects.Count() > 0)
            {
                screen.print(StartingX + 2, nextAvailablePosition, "Status Effects:");
                StringBuilder statusEffects = new StringBuilder();
                foreach (string s in m_player.StatusEffects)
                {
                    statusEffects.Append(s + " ");
                }

                // TODO - What happens if this is more then 2 lines worth?
                int statusEfectLength = screen.printRect(StartingX + 2, nextAvailablePosition + 1, InfoWidth - 4, 2, statusEffects.ToString());

                nextAvailablePosition += 2 + statusEfectLength;
            }

            if (m_monstersNearby.Count > 0)
            {
                string countAmount = m_monstersNearby.Count < 8 ? m_monstersNearby.Count.ToString() : "8+";
                screen.print(StartingX + 2, nextAvailablePosition, string.Format("Nearby Enemies ({0}):", countAmount));
                for (int i = 0; i < m_monstersNearby.Count; ++i)
                {
                    ICharacter currentMonster = m_monstersNearby[i];
                    screen.printEx(StartingX + 12, nextAvailablePosition + 1 + i, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, currentMonster.Name);
                    for (int j = 0; j < HealthBarLength(currentMonster, true); ++j)
                        screen.setCharBackground(StartingX + 2 + j, nextAvailablePosition + 1 + i, EnemyHealthBarColor(currentMonster));
                }
                nextAvailablePosition += 2;
            }

            if (Preferences.Instance.DebuggingMode)
            {
                string turnCount = m_turnCount.ToString();
                screen.print(54, 40, "Turn Count - " + turnCount);

                string level = (m_currentLevel + 1).ToString();
                screen.print(54, 41, "Level - " + level);

                string position = m_player.Position.ToString();
                screen.print(54, 42, position);

                string fps = TCODSystem.getFps().ToString();
                screen.print(54, 43, fps);
            }
        }
    }
}
