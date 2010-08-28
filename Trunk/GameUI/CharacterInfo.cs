using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    internal class CharacterInfo : PainterBase
    {
        public CharacterInfo()
        {
            m_currentLevel = -1;
            m_colorHelper = new DialogColorHelper();
            MapCursorEnabled = false;
            CursorSpot = Point.Invalid;
        }

        private const int StartingX = UIHelper.MapWidth;
        private const int InfoWidth = UIHelper.CharInfoWidth;
        private const int InfoHeight = UIHelper.CharInfoHeight;

        private const int SectionCenter = StartingX + (InfoWidth / 2);

        private const int BarLength = 23;

        private int m_currentLevel;
        private List<ICharacter> m_monstersNearby;
        private int m_turnCount;
        private bool m_inDanger;
        private DialogColorHelper m_colorHelper;

        internal bool MapCursorEnabled { get; set; }
        internal Point CursorSpot { get; set; }

        private int m_currentHealth;
        private int m_maxHealth;
        private int m_currentStamina;
        private int m_maxStamina;
        private int m_currentHP;
        private int m_maxHP;
        private int m_currentMP;
        private int m_maxMP;
        private int m_maxPossibleMP;
        private Point m_position;
        private string m_name;
        private IWeapon m_currentWeapon;
        private List<IStatusEffect> m_statusEffects;
        private int m_skillPoints;

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            IPlayer player = engine.Player;
            m_name = player.Name;
            m_position = player.Position;

            m_currentHealth = player.CurrentHealth;
            m_maxHealth = player.MaxHealth;
            m_currentStamina = player.CurrentStamina;
            m_maxStamina = player.MaxStamina;
            m_currentHP = player.CurrentHP;
            m_maxHP = player.MaxHP;

            m_currentMP = player.CurrentMP;
            m_maxMP = player.MaxMP;
            m_maxPossibleMP = player.MaxPossibleMP;

            m_currentWeapon = player.CurrentWeapon;
            m_statusEffects = player.StatusEffects.ToList();
            m_skillPoints = player.SkillPoints;

            m_currentLevel = engine.CurrentLevel;
            m_monstersNearby = engine.GameState.MonstersInPlayerLOS().OrderBy(x => PointDirectionUtils.LatticeDistance(x.Position, m_position)).ToList();
            m_turnCount = engine.TurnCount;
            m_inDanger = engine.GameState.CurrentOrRecentDanger();
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

        private TCODColor PlayerHealthBarColorAtPosition(int position)
        {
            int healthPortionOfPlayerHPBarLength = StandardBarLength(m_maxHealth, m_maxHP);
            int staminaPortionOfPlayerHPBarLength = BarLength - healthPortionOfPlayerHPBarLength;

            if (position < healthPortionOfPlayerHPBarLength)
            {
                double percentage = ((double)m_currentHealth / (double)m_maxHealth) * 100;
                if (position >= CalculateBarLength(m_currentHealth, m_maxHealth, healthPortionOfPlayerHPBarLength))
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
                double percentage = ((double)m_currentStamina / (double)m_maxStamina) * 100;
                if (position - healthPortionOfPlayerHPBarLength >= CalculateBarLength(m_currentStamina, m_maxStamina, staminaPortionOfPlayerHPBarLength))
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

        private TCODColor PlayerManaBarColor(int position)
        {
            int enabledPortionOfMPBarLength = StandardBarLength(m_maxMP, m_maxPossibleMP);

            if (position < enabledPortionOfMPBarLength)
            {
                double percentage = ((double)m_currentMP / (double)m_maxMP) * 100;
                if (position >= CalculateBarLength(m_currentMP, m_maxMP, enabledPortionOfMPBarLength))
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
            screen.printEx(SectionCenter, 1, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_name);

            // The 7 and 18 here are psydo-magical, since they place the text overlap just so it fits and doesn't go over for sane values
            string healthString = string.Format("Hlth {0}/{1}", m_currentHealth, m_maxHealth);
            string staminaString = string.Format("Sta {0}/{1}", m_currentStamina, m_maxStamina);
            screen.printEx(StartingX + 7, 2, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, healthString);
            screen.printEx(StartingX + 18, 2, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, staminaString);
            for (int j = 0; j < BarLength; ++j)
                screen.setCharBackground(StartingX + 2 + j, 2, PlayerHealthBarColorAtPosition(j));

            string manaString = string.Format("Mana {0}/{1}", m_currentMP, m_maxMP);
            screen.printEx(StartingX + 13, 3, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, manaString);
            for (int j = 0; j < BarLength; ++j)
                screen.setCharBackground(StartingX + 2 + j, 3, PlayerManaBarColor(j));

            int nextAvailablePosition = 6;

            string skillPointString = "Skill Points: " + m_skillPoints;
            screen.print(StartingX + 2, nextAvailablePosition, skillPointString);
            nextAvailablePosition += 2;

            string needsLoadedString = !m_currentWeapon.IsRanged || m_currentWeapon.IsLoaded ? "" : "(empty)";
            int linesTaken = screen.printRect(StartingX + 2, nextAvailablePosition, UIHelper.ScreenWidth - StartingX - 3, 5, string.Format("Weapon: {0} {1}", m_currentWeapon.DisplayName, needsLoadedString));
            nextAvailablePosition += linesTaken + 1;

            m_colorHelper.SaveColors(screen);
            if (m_statusEffects.Count() > 0)
            {
                screen.print(StartingX + 2, nextAvailablePosition, "Status Effects:");
                int currentX = StartingX + 2 + 1 + 15;
                foreach (IStatusEffect s in m_statusEffects)
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

            m_colorHelper.SaveColors(screen);
            if (m_monstersNearby.Count > 0)
            {
                string countAmount = m_monstersNearby.Count < 8 ? m_monstersNearby.Count.ToString() : "8+";
                screen.print(StartingX + 2, nextAvailablePosition, string.Format("Nearby Enemies ({0}):", countAmount));
                
                // Show at most 8 monsters
                int numberOfMonstersToShow = m_monstersNearby.Count > 8 ? 8 : m_monstersNearby.Count;
                for (int i = 0; i < numberOfMonstersToShow; ++i)
                {
                    ICharacter currentMonster = m_monstersNearby[i];
                    if (MapCursorEnabled)
                    {
                        if (currentMonster.Position == CursorSpot)
                            screen.setForegroundColor(TCODColor.darkYellow);
                        else
                            screen.setForegroundColor(UIHelper.ForegroundColor);
                    }

                    screen.printEx(StartingX + 12, nextAvailablePosition + 1 + i, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, currentMonster.Name);
                    for (int j = 0; j < HealthBarLength(currentMonster, true); ++j)
                        screen.setCharBackground(StartingX + 2 + j, nextAvailablePosition + 1 + i, EnemyHealthBarColor(currentMonster));
                }
                nextAvailablePosition += 2;
            }
            m_colorHelper.ResetColors(screen);

            if (Preferences.Instance.DebuggingMode)
            {
                screen.print(54, 39, "Turn Count - " + m_turnCount.ToString());

                screen.print(54, 40, "Danger - " + m_inDanger.ToString());

                string level = (m_currentLevel + 1).ToString();
                screen.print(54, 41, "Level - " + level);

                string position = m_position.ToString();
                screen.print(54, 42, position);

                string fps = TCODSystem.getFps().ToString();
                screen.print(54, 43, fps);
            }
        }
    }
}
