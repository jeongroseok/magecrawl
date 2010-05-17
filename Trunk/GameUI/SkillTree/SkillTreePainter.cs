using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreePainter : MapPainterBase
    {
        private Dictionary<string, SkillTreeTab> m_skillTreeTabs;
        private TCODConsole m_offscreenConsole;
        private List<string> m_tabOrderingList = new List<string>() { "Arcane", "Fire", "Light", "Martial" };
        //private List<string> m_tabOrderingList = new List<string>() { "Air", "Arcane", "Darkness", "Earth", "Fire", "Light", "Martial", "Water" };
        private string m_defaultTab = "Arcane";
        //private string m_defaultTab = "Air";
        private string m_currentTabName;
        private IGameEngine m_engine;

        // Set when we need to redraw offscreen buffer
        private bool m_dirtyFrame;
     
        internal List<ISkill> NewlySelectedSkills { get; private set; }

        private const int UpperLeft = 5;
        private const int ScreenWidth = 70;
        private const int ScreenHeight = 50;
        private static Point SkillTreeScreenCenter = new Point(((ScreenWidth - 1) / 2), ((ScreenHeight - 1) / 2));

        internal SkillTreePainter()
        {
            m_enabled = false;
            CursorPosition = Point.Invalid;
            NewlySelectedSkills = new List<ISkill>();
            m_currentTabName = m_defaultTab;
            m_dirtyFrame = true;

            m_skillTreeTabs = new Dictionary<string, SkillTreeTab>();
            //m_skillTreeTabs.Add("Air", new SkillTreeTab("AirSkillTree.dat"));
            m_skillTreeTabs.Add("Arcane", new SkillTreeTab("ArcaneSkillTree.dat"));
            //m_skillTreeTabs.Add("Darkness", new SkillTreeTab("DarknessSkillTree.dat"));
            //m_skillTreeTabs.Add("Earth", new SkillTreeTab("EarthSkillTree.dat"));
            m_skillTreeTabs.Add("Fire", new SkillTreeTab("FireSkillTree.dat"));
            m_skillTreeTabs.Add("Light", new SkillTreeTab("LightSkillTree.dat"));
            m_skillTreeTabs.Add("Martial", new SkillTreeTab("MartialSkillTree.dat"));
            //m_skillTreeTabs.Add("Water", new SkillTreeTab("WaterSkillTree.dat"));

            // Calculate the max width/height of all tabs so we can get the offsecreen surface the right size
            int maxWidth = -1;
            int maxHeight = -1;
            foreach (SkillTreeTab tab in m_skillTreeTabs.Values)
            {
                maxWidth = Math.Max(maxWidth, tab.Width);
                maxHeight = Math.Max(maxHeight, tab.Height);
            }
            
            // The offconsole width/height here are completely wrong, but "good enough" being too big
            //m_offscreenConsole = new TCODConsole(maxWidth * 2 + SkillTreeTab.ExplainPopupWidth, maxHeight * 2 + SkillTreeTab.ExplainPopupHeight);
            m_offscreenConsole = new TCODConsole(maxWidth * SkillTreeTab.ExplainPopupWidth + 1, maxHeight *  SkillTreeTab.ExplainPopupHeight + 1);
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
                    CurrentTab.Draw(m_offscreenConsole, m_engine, GetAllSelectedSkill(), CursorPosition);
                    m_dirtyFrame = false;
                }

                int lowX = CursorPosition.X - (ScreenWidth / 2);
                int lowY = CursorPosition.Y - (ScreenHeight / 2) + SkillTreeTab.ExplainPopupHeight;
                screen.printFrame(UpperLeft, UpperLeft, ScreenWidth, ScreenHeight, true, TCODBackgroundFlag.Set, "Skill Tree");

                TCODConsole.blit(m_offscreenConsole, lowX, lowY, ScreenWidth - 2, ScreenHeight - 2, screen, UpperLeft + 1, UpperLeft + 1);

                DrawTabBar(screen);

                // Draw cursor
                screen.setCharBackground(SkillTreeScreenCenter.X + UpperLeft + 2, SkillTreeScreenCenter.Y + UpperLeft + 2, TCODColor.darkGrey);

                // For debugging
                //screen.print(50, 50, CursorPosition.ToString());
            }
        }

        private void DrawTabBar(TCODConsole screen)
        {
            const int HorizontalTabOffset = 2;
            screen.rect(UpperLeft + 1, UpperLeft + 1, ScreenWidth - 2, HorizontalTabOffset - 1, true);

            screen.putChar(UpperLeft, UpperLeft + HorizontalTabOffset, (int)TCODSpecialCharacter.TeeEast);
            screen.putChar(UpperLeft + ScreenWidth - 1, UpperLeft + HorizontalTabOffset, (int)TCODSpecialCharacter.TeeWest);
            screen.hline(UpperLeft + 1, UpperLeft + HorizontalTabOffset, ScreenWidth - 2);

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
                    foreach(ISkill skill in GetAllSelectedSkill())
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
                    if (!SkillTreeModelHelpers.HasUnmetDependencies(GetAllSelectedSkill(), CurrentTab.SkillSquareCursorIsOver(CursorPosition)))
                    {
                        NewlySelectedSkills.Add(selected);
                        m_dirtyFrame = true;
                    }
                }
            }
        }

        public override void UpdateFromNewData (IGameEngine engine, Point mapUpCorner, Point centerPosition)
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
