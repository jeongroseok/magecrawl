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
            m_colorHelper = new DialogColorHelper();
        }

        private const int StartingX = UIHelper.MapWidth;
        private const int InfoWidth = UIHelper.CharInfoWidth;
        private const int InfoHeight = UIHelper.CharInfoHeight;

        private const int SectionCenter = StartingX + (InfoWidth / 2);

        private const int BarLength = 23;

        private IPlayer m_player;
        private int m_currentLevel;
        private List<ICharacter> m_monstersNearby;
        private int m_turnCount;
        private bool m_inDanger;
        private DialogColorHelper m_colorHelper;

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_player = engine.Player;
            m_currentLevel = engine.CurrentLevel;
            m_monstersNearby = engine.MonstersInPlayerLOS().OrderBy(x => PointDirectionUtils.LatticeDistance(x.Position, m_player.Position)).ToList();
            m_turnCount = engine.TurnCount;
            m_inDanger = engine.CurrentOrRecentDanger();
        }

        private static int CalculateBarLength(double part, double whole, double length)
        {
            return (int)Math.Round((part / whole) * length);
        }

        private static int StandardBarLength(double part, double whole)
        {
            return CalculateBarLength(part, whole, BarLength);
        }

        private int HealthBarLength(ICharacter character, bool fuzzy)
        {
            if (fuzzy)
            {
                int value = (int)Math.Floor(((double)character.CurrentHP / (double)character.MaxHP) * BarLength);
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
                return StandardBarLength(character.CurrentHP, character.MaxHP);
        }

        private int ManaBarLength(IPlayer character)
        {
            return StandardBarLength(character.CurrentMP, character.MaxMP);
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

        private TCODColor PlayerHealthBarColorAtPosition(IPlayer player, int position)
        {
            int HealthPortionOfPlayerHPBarLength = StandardBarLength(player.MaxHealth, player.MaxHP);
            int StaminaPortionOfPlayerHPBarLength = BarLength - HealthPortionOfPlayerHPBarLength;

            if (position < HealthPortionOfPlayerHPBarLength)
            {
                double percentage = ((double)player.CurrentHealth / (double)player.MaxHealth) * 100;
                if (position >= CalculateBarLength(player.CurrentHealth, player.MaxHealth, HealthPortionOfPlayerHPBarLength))
                    return TCODColor.black;
                if (percentage > 95)
                    return TCODColor.orange.Divide(2);
                else if (percentage > 70)
                    return TCODColor.orange.Divide(2.5);
                else if (percentage > 35)
                    return TCODColor.orange.Divide(3);
                else
                    return TCODColor.darkOrange.Divide(2);
            }
            else
            {
                double percentage = ((double)player.CurrentStamina / (double)player.MaxStamina) * 100;
                if (position - HealthPortionOfPlayerHPBarLength >= CalculateBarLength(player.CurrentStamina, player.MaxStamina, StaminaPortionOfPlayerHPBarLength))
                    return TCODColor.black;
                if (percentage > 95)
                    return TCODColor.red.Divide(2);
                else if (percentage > 70)
                    return TCODColor.red.Divide(2.5);
                else if (percentage > 35)
                    return TCODColor.red.Divide(3);
                else
                    return TCODColor.darkRed.Divide(2);
            }
        }

        private TCODColor PlayerManaBarColor(IPlayer player, int position)
        {
            int EnabledPortionOfMPBarLength = StandardBarLength(player.MaxMP, player.MaxPossibleMP);

            if (position < EnabledPortionOfMPBarLength)
            {
                double percentage = ((double)player.CurrentMP / (double)player.MaxMP) * 100;
                if (position >= CalculateBarLength(player.CurrentMP, player.MaxMP, EnabledPortionOfMPBarLength))
                    return TCODColor.black;
                if (percentage > 95)
                    return TCODColor.blue.Divide(1.5);
                else if (percentage > 70)
                    return TCODColor.blue.Divide(2);
                else if (percentage > 35)
                    return TCODColor.blue.Divide(3);
                else
                    return TCODColor.blue.Divide(4);
            }
            else
            {
                return TCODColor.grey;
            }
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            screen.printFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.printEx(SectionCenter, 1, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_player.Name);

            // The 7 and 18 here are psydo-magical, since they place the text overlap just so it fits and doesn't go over for sane values
            string healthString = string.Format("Hlth {0}/{1}", m_player.CurrentHealth, m_player.MaxHealth);
            string staminaString = string.Format("Sta {0}/{1}", m_player.CurrentStamina, m_player.MaxStamina);
            screen.printEx(StartingX + 7, 2, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, healthString);
            screen.printEx(StartingX + 18, 2, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, staminaString);
            for (int j = 0; j < BarLength; ++j)
                screen.setCharBackground(StartingX + 2 + j, 2, PlayerHealthBarColorAtPosition(m_player, j));

            string magicString = string.Format("Magic {0}/{1}", m_player.CurrentMP, m_player.MaxMP);
            screen.printEx(StartingX + 12, 3, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, magicString);
            for (int j = 0; j < BarLength; ++j)
                screen.setCharBackground(StartingX + 2 + j, 3, PlayerManaBarColor(m_player, j ));

            int nextAvailablePosition = 6;

            string skillPointString = "Skill Points: " + m_player.SkillPoints;
            screen.print(StartingX + 2, nextAvailablePosition, skillPointString);
            nextAvailablePosition += 2;

            string needsLoadedString = m_player.CurrentWeapon.IsLoaded ? "" : "(empty)";
            screen.print(StartingX + 2, nextAvailablePosition, string.Format("Weapon: {0}{1}", m_player.CurrentWeapon.DisplayName, needsLoadedString));
            nextAvailablePosition += 2;          

            m_colorHelper.SaveColors(screen);
            if (m_player.StatusEffects.Count() > 0)
            {
                screen.print(StartingX + 2, nextAvailablePosition, "Status Effects:");
                int currentX = StartingX + 2 + 1 + 15;
                foreach (IStatusEffect s in m_player.StatusEffects)
                {
                    if (currentX + s.Name.Length >= UIHelper.ScreenWidth)
                    {
                        currentX = StartingX + 2;
                        nextAvailablePosition++;
                    }
                    screen.setForegroundColor(s.IsPositiveEffect ? TCODColor.darkGreen : TCODColor.darkRed);
                    screen.print(currentX, nextAvailablePosition, s.Name);
                    currentX += s.Name.Length + 1;
                }
                nextAvailablePosition += 2;
            }
            m_colorHelper.ResetColors(screen);

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
                screen.print(54, 39, "Turn Count - " + m_turnCount.ToString());

                screen.print(54, 40, "Danger - " + m_inDanger.ToString());

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
