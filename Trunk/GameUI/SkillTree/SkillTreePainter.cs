using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreePainter : MapPainterBase
    {
        private Dictionary<string, SkillTreeTab> m_skillTreeTabs;
        private List<string> m_tabOrderingList = new List<string>() { "Arcane", "Fire", "Light", "Martial", "Attributes"};
        //private List<string> m_tabOrderingList = new List<string>() { "Air", "Arcane", "Earth", "Fire", "Light", "Martial", "Water" };
        private string m_defaultTab = "Arcane";
        //private string m_defaultTab = "Air";
        private string m_currentTabName;
        private IGameEngine m_engine;

        // Set when we need to redraw offscreen buffer
        private bool m_dirtyFrame;
        private TCODConsole m_offscreen;
     
        internal List<ISkill> NewlySelectedSkills { get; private set; }

        public const int UpperLeft = 5;
        public const int SkillTreeWidth = UIHelper.ScreenWidth - 2 * UpperLeft;
        public const int SkillTreeHeight = UIHelper.ScreenHeight - 2 * UpperLeft;
        public static Point SkillTreeCursorPosition = new Point(((SkillTreeWidth - 1) / 2) + UpperLeft + 2, ((SkillTreeHeight - 1) / 2) + UpperLeft + 2);

        internal SkillTreePainter()
        {
            m_enabled = false;
            CursorPosition = Point.Invalid;
            NewlySelectedSkills = new List<ISkill>();
            m_currentTabName = m_defaultTab;
            m_dirtyFrame = true;
            m_offscreen = new TCODConsole(UIHelper.ScreenWidth, UIHelper.ScreenHeight);

            m_skillTreeTabs = new Dictionary<string, SkillTreeTab>();
            //m_skillTreeTabs.Add("Air", new SkillTreeTab("AirSkillTree.xml"));
            m_skillTreeTabs.Add("Arcane", new SkillTreeTab("ArcaneSkillTree.xml"));
            //m_skillTreeTabs.Add("Earth", new SkillTreeTab("EarthSkillTree.xml"));
            m_skillTreeTabs.Add("Fire", new SkillTreeTab("FireSkillTree.xml"));
            m_skillTreeTabs.Add("Light", new SkillTreeTab("LightSkillTree.xml"));
            m_skillTreeTabs.Add("Martial", new SkillTreeTab("MartialSkillTree.xml"));
            m_skillTreeTabs.Add("Attributes", new SkillTreeTab("AttributeSkillTree.xml"));
            //m_skillTreeTabs.Add("Water", new SkillTreeTab("WaterSkillTree.xml"));

            // Calculate the max width/height of all tabs so we can get the offsecreen surface the right size
            int maxWidth = -1;
            int maxHeight = -1;
            foreach (SkillTreeTab tab in m_skillTreeTabs.Values)
            {
                maxWidth = Math.Max(maxWidth, tab.Width);
                maxHeight = Math.Max(maxHeight, tab.Height);
            }            
        }

        private List<ISkill> GetAllSelectedSkill()
        {
            List<ISkill> list = new List<ISkill>();
            list.AddRange(NewlySelectedSkills);
            list.AddRange(m_engine.Player.Skills);
            return list;
        }

        private SkillTreeTab CurrentTab
        {
            get
            {
                return m_skillTreeTabs[m_currentTabName];
            }
        }

        public void IncrementCurrentTab()
        {
            int positionInList = m_tabOrderingList.FindIndex(x => x == m_currentTabName);
            positionInList++;
            if (positionInList >= m_tabOrderingList.Count)
                positionInList = 0;
            m_currentTabName = m_tabOrderingList[positionInList];
            CursorPosition = CurrentTab.DefaultCurorPosition;
            m_dirtyFrame = true;
        }

        public void DecrementCurrentTab()
        {
            int positionInList = m_tabOrderingList.FindIndex(x => x == m_currentTabName);
            positionInList--;
            if (positionInList < 0)
                positionInList = m_tabOrderingList.Count - 1;
            m_currentTabName = m_tabOrderingList[positionInList];
            CursorPosition = CurrentTab.DefaultCurorPosition;
            m_dirtyFrame = true;
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            if (Enabled)
            {
                if (m_dirtyFrame)
                {
                    m_offscreen.printFrame(UpperLeft, UpperLeft, SkillTreeWidth, SkillTreeHeight, true, TCODBackgroundFlag.Set, "Skill Tree");

                    CurrentTab.Draw(m_offscreen, m_engine, GetAllSelectedSkill(), NewlySelectedSkills, CursorPosition);
                    DrawTabBar(m_offscreen);
                    DrawSkillPointTotalFrame(m_offscreen);

                    // Draw cursor
                    m_offscreen.setCharBackground(SkillTreeCursorPosition.X, SkillTreeCursorPosition.Y, TCODColor.darkGrey);
                    m_dirtyFrame = false;
                }
                TCODConsole.blit(m_offscreen, 5, 5, SkillTreeWidth, SkillTreeHeight, screen, 5, 5);
            }
        }

        private void DrawSkillPointTotalFrame(TCODConsole screen)
        {
            screen.printFrame(5, 48, 16, 7, true);
            screen.putChar(5, 48, (int)TCODSpecialCharacter.TeeEast);
            screen.putChar(20, 54, (int)TCODSpecialCharacter.TeeNorth);
            screen.print(6, 49, "Skill Points:");
            screen.print(6, 51, "SP Earned:" + m_engine.Player.SkillPoints.ToString());
            int selectedSkillCost = NewlySelectedSkills.Sum(x => x.Cost);
            screen.print(6, 52, "SP Spent: " + selectedSkillCost);
            screen.print(6, 53, "SP Left:  " + (m_engine.Player.SkillPoints - selectedSkillCost).ToString());
        }

        private void DrawTabBar(TCODConsole screen)
        {
            const int HorizontalTabOffset = 2;
            screen.rect(UpperLeft + 1, UpperLeft + 1, SkillTreeWidth - 2, HorizontalTabOffset - 1, true);

            screen.putChar(UpperLeft, UpperLeft + HorizontalTabOffset, (int)TCODSpecialCharacter.TeeEast);
            screen.putChar(UpperLeft + SkillTreeWidth - 1, UpperLeft + HorizontalTabOffset, (int)TCODSpecialCharacter.TeeWest);
            screen.hline(UpperLeft + 1, UpperLeft + HorizontalTabOffset, SkillTreeWidth - 2);

            int x = UpperLeft + 2;
            int y = UpperLeft + HorizontalTabOffset - 1;
            foreach (string name in m_skillTreeTabs.Keys)
            {
                int xOffset = name == "Water" ? 1 : 0;
                screen.print(x + xOffset, y, name);
                x += name.Length + 1;

                if (name != "Water")
                {
                    screen.putChar(x, y, (int)TCODSpecialCharacter.VertLine);

                    // This is a bit of a hack. We don't want to overwrite the color'ed background title of the frame, so we check first
                    // This can break if libtcod changes that background title color
                    if (!screen.getCharBackground(x, y - 1).Equal(new TCODColor(211, 211, 211)))
                        screen.putChar(x, y - 1, (int)TCODSpecialCharacter.TeeSouth);

                    screen.putChar(x, y + 1, (int)TCODSpecialCharacter.TeeNorth);
                }
                x += 2;

                if (m_currentTabName == name)
                {
                    int lengthReductionToDueTee = name == "Water" ? 0 : 2;
                    for (int i = x - 4 - name.Length; i < x - lengthReductionToDueTee; i++)
                    {
                        screen.setCharBackground(i, y, TCODColor.grey);
                    }
                }
            }
        }

        internal void SelectSquare()
        {
            ISkill selected = CurrentTab.SkillCursorIsOver(m_engine, CursorPosition);
            if (selected != null)
            {
                // Don't allow skills to be deselected once chosen.
                if (m_engine.Player.Skills.Contains(selected))
                    return;

                if (NewlySelectedSkills.Contains(selected)) // Deselecting
                {
                    bool somebodyHasDependencyOnLeavingSkill = false;
                    foreach (ISkill skill in GetAllSelectedSkill())
                    {
                        if (SkillTreeModelHelpers.HasDependencyOn(m_skillTreeTabs.Values, skill.Name, selected.Name))
                            somebodyHasDependencyOnLeavingSkill = true;
                    }
                    if (!somebodyHasDependencyOnLeavingSkill)
                    {
                        NewlySelectedSkills.Remove(selected);
                        m_dirtyFrame = true;
                    }
                }
                else // Selecting
                {
                    bool hasEnoughSkillPointsToSelectThisAsWell = NewlySelectedSkills.Sum(x => x.Cost) + selected.Cost <= m_engine.Player.SkillPoints;
                    bool hasDependenciesMet = !SkillTreeModelHelpers.HasUnmetDependencies(GetAllSelectedSkill(), CurrentTab.SkillSquareCursorIsOver(CursorPosition));
                    if (hasEnoughSkillPointsToSelectThisAsWell && hasDependenciesMet)
                    {
                        NewlySelectedSkills.Add(selected);
                        m_dirtyFrame = true;
                    }
                }
            }
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_engine = engine;
        }

        private Point m_cursorPosition;
        internal Point CursorPosition 
        {
            get
            {
                return m_cursorPosition;                
            }
            set
            {
                m_cursorPosition = value;
                m_dirtyFrame = true;
            }
        }

        private bool m_enabled;
        internal bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
                m_currentTabName = m_defaultTab;
                CursorPosition = CurrentTab.DefaultCurorPosition;
                NewlySelectedSkills.Clear();
                m_dirtyFrame = true;
            }
        }
    }
}
