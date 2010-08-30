using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameUI.Utilities;
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
            SetupCachedColors();
        }

        private static void SetupCachedColors()
        {
            ColorCache.Instance["Black"] = TCODColor.black;
            ColorCache.Instance["RedDiv2"] = TCODColor.red.Divide(2);
            ColorCache.Instance["RedDiv2.5"] = TCODColor.red.Divide(2.5);
            ColorCache.Instance["RedDiv3"] = TCODColor.red.Divide(3);
            ColorCache.Instance["DarkRedDiv2"] = TCODColor.darkRed.Divide(2);

            ColorCache.Instance["BlueDiv1.5"] = TCODColor.blue.Divide(1.5);
            ColorCache.Instance["BlueDiv2"] = TCODColor.blue.Divide(2);
            ColorCache.Instance["BlueDiv3"] = TCODColor.blue.Divide(3);
            ColorCache.Instance["BlueDiv4"] = TCODColor.blue.Divide(4);

            ColorCache.Instance["DarkBlueDiv2"] = TCODColor.darkBlue.Divide(2);
            ColorCache.Instance["DarkGreenDiv2"] = TCODColor.darkGreen.Divide(2);
            ColorCache.Instance["DarkOrangeDiv2"] = TCODColor.darkOrange.Divide(2);
            ColorCache.Instance["DarkRedDiv2"] = TCODColor.darkRed.Divide(2);

            ColorCache.Instance["OrangeDiv2"] = TCODColor.orange.Divide(2);
            ColorCache.Instance["OrangeDiv2.5"] = TCODColor.orange.Divide(2.5);
            ColorCache.Instance["OrangeDiv3"] = TCODColor.orange.Divide(3);

            ColorCache.Instance["DarkGreen"] = TCODColor.darkGreen;
            ColorCache.Instance["DarkRed"] = TCODColor.darkRed;
            ColorCache.Instance["DarkYellow"] = TCODColor.darkYellow;

            ColorCache.Instance["Grey"] = TCODColor.grey;
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
        private string m_healthString;
        private string m_staminaString;
        private string m_manaString;
        private string m_weaponString;
        private string m_nearbyEnemyString;

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

            m_healthString = string.Format("Hlth {0}/{1}", m_currentHealth, m_maxHealth);
            m_staminaString = string.Format("Sta {0}/{1}", m_currentStamina, m_maxStamina);
            m_manaString = string.Format("Mana {0}/{1}", m_currentMP, m_maxMP);

            string needsLoadedString = !m_currentWeapon.IsRanged || m_currentWeapon.IsLoaded ? "" : "(empty)";
            m_weaponString = string.Format("Weapon: {0} {1}", m_currentWeapon.DisplayName, needsLoadedString);

            m_monstersNearby = engine.GameState.MonstersInPlayerLOS().OrderBy(x => PointDirectionUtils.LatticeDistance(x.Position, m_position)).ToList();

            string countAmount = m_monstersNearby.Count < 8 ? m_monstersNearby.Count.ToString() : "8+";
            m_nearbyEnemyString = string.Format("Nearby Enemies ({0}):", countAmount);

            m_currentLevel = engine.CurrentLevel;
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
                return ColorCache.Instance["DarkBlueDiv2"];
            else if (percentage > 70)
                return ColorCache.Instance["DarkGreenDiv2"];
            else if (percentage > 35)
                return ColorCache.Instance["DarkOrangeDiv2"];
            else
                return ColorCache.Instance["DarkRedDiv2"];
        }

        private TCODColor PlayerHealthBarColorAtPosition(int position)
        {
            int healthPortionOfPlayerHPBarLength = StandardBarLength(m_maxHealth, m_maxHP);
            int staminaPortionOfPlayerHPBarLength = BarLength - healthPortionOfPlayerHPBarLength;

            if (position < healthPortionOfPlayerHPBarLength)
            {
                double percentage = ((double)m_currentHealth / (double)m_maxHealth) * 100;
                if (position >= CalculateBarLength(m_currentHealth, m_maxHealth, healthPortionOfPlayerHPBarLength))
                    return ColorCache.Instance["Black"];
                if (percentage > 95)
                    return ColorCache.Instance["OrangeDiv2"];
                else if (percentage > 70)
                    return ColorCache.Instance["OrangeDiv2.5"];
                else if (percentage > 35)
                    return ColorCache.Instance["OrangeDiv3"];
                else
                    return ColorCache.Instance["DarkOrangeDiv2"];
            }
            else
            {
                double percentage = ((double)m_currentStamina / (double)m_maxStamina) * 100;
                if (position - healthPortionOfPlayerHPBarLength >= CalculateBarLength(m_currentStamina, m_maxStamina, staminaPortionOfPlayerHPBarLength))
                    return ColorCache.Instance["Black"];
                if (percentage > 95)
                    return ColorCache.Instance["RedDiv2"];
                else if (percentage > 70)
                    return ColorCache.Instance["RedDiv2.5"];
                else if (percentage > 35)
                    return ColorCache.Instance["RedDiv3"];
                else
                    return ColorCache.Instance["DarkRedDiv2"];
            }
        }

        private TCODColor PlayerManaBarColor(int position)
        {
            int enabledPortionOfMPBarLength = StandardBarLength(m_maxMP, m_maxPossibleMP);

            if (position < enabledPortionOfMPBarLength)
            {
                double percentage = ((double)m_currentMP / (double)m_maxMP) * 100;
                if (position >= CalculateBarLength(m_currentMP, m_maxMP, enabledPortionOfMPBarLength))
                    return ColorCache.Instance["Black"];
                if (percentage > 95)
                    return ColorCache.Instance["BlueDiv1.5"];
                else if (percentage > 70)
                    return ColorCache.Instance["BlueDiv2"];
                else if (percentage > 35)
                    return ColorCache.Instance["BlueDiv3"];
                else
                    return ColorCache.Instance["BlueDiv4"];
            }
            else
            {
                return ColorCache.Instance["Grey"];
            }
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            screen.printFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.printEx(SectionCenter, 1, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_name);

            // The 7 and 18 here are psydo-magical, since they place the text overlap just so it fits and doesn't go over for sane values
            screen.printEx(StartingX + 7, 2, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, m_healthString);
            screen.printEx(StartingX + 18, 2, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, m_staminaString);
            for (int j = 0; j < BarLength; ++j)
                screen.setCharBackground(StartingX + 2 + j, 2, PlayerHealthBarColorAtPosition(j));

            screen.printEx(StartingX + 13, 3, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_manaString);
            for (int j = 0; j < BarLength; ++j)
                screen.setCharBackground(StartingX + 2 + j, 3, PlayerManaBarColor(j));

            int nextAvailablePosition = 6;

            string skillPointString = "Skill Points: " + m_skillPoints;
            screen.print(StartingX + 2, nextAvailablePosition, skillPointString);
            nextAvailablePosition += 2;

            int linesTaken = screen.printRect(StartingX + 2, nextAvailablePosition, UIHelper.ScreenWidth - StartingX - 3, 5, m_weaponString);
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
                    screen.setForegroundColor(s.IsPositiveEffect ? ColorCache.Instance["DarkGreen"] : ColorCache.Instance["DarkRed"] );
                    screen.print(currentX, nextAvailablePosition, s.Name);
                    currentX += s.Name.Length + 1;
                }
                nextAvailablePosition += 2;
            }
            m_colorHelper.ResetColors(screen);

            m_colorHelper.SaveColors(screen);
            if (m_monstersNearby.Count > 0)
            {
                screen.print(StartingX + 2, nextAvailablePosition, m_nearbyEnemyString);
                
                // Show at most 8 monsters
                int numberOfMonstersToShow = m_monstersNearby.Count > 8 ? 8 : m_monstersNearby.Count;
                for (int i = 0; i < numberOfMonstersToShow; ++i)
                {
                    ICharacter currentMonster = m_monstersNearby[i];
                    if (MapCursorEnabled)
                    {
                        if (currentMonster.Position == CursorSpot)
                            screen.setForegroundColor(ColorCache.Instance["DarkYellow"]);
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
