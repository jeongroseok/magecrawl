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
        private string m_defaultTab = "Arcane";
        private string m_currentTabName;
        private IGameEngine m_engine;

        internal Point CursorPosition { get; set; }        
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

            m_skillTreeTabs = new Dictionary<string, SkillTreeTab>();
            m_skillTreeTabs.Add("Arcane", new SkillTreeTab("ArcaneSkillTree.dat"));

            // Calculate the max width/height of all tabs so we can get the offsecreen surface the right size
            int maxWidth = -1;
            int maxHeight = -1;
            foreach (SkillTreeTab tab in m_skillTreeTabs.Values)
            {
                maxWidth = Math.Max(maxWidth, tab.Width);
                maxHeight = Math.Max(maxHeight, tab.Height);
            }
            m_offscreenConsole = new TCODConsole(maxWidth + SkillTreeTab.ExplainPopupWidth + 1, maxHeight + SkillTreeTab.ExplainPopupHeight + 1);
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

        public override void DrawNewFrame (TCODConsole screen)
        {
            if (Enabled)
            {
                CurrentTab.Draw(m_offscreenConsole, m_engine, GetAllSelectedSkill(), CursorPosition);

                int lowX = CursorPosition.X - (ScreenWidth / 2);
                int lowY = CursorPosition.Y - (ScreenHeight / 2);
                screen.printFrame(UpperLeft, UpperLeft, ScreenWidth, ScreenHeight, true, TCODBackgroundFlag.Set, "Skill Tree");

                TCODConsole.blit(m_offscreenConsole, lowX, lowY, ScreenWidth - 2, ScreenHeight - 2, screen, UpperLeft + 1, UpperLeft + 1);

                // Draw cursor
                screen.setCharBackground(SkillTreeScreenCenter.X + UpperLeft + 2, SkillTreeScreenCenter.Y + UpperLeft + 2, TCODColor.darkGrey);

                // For debugging
                // screen.print(50, 50, m_cursorPosition.ToString());
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
                        NewlySelectedSkills.Remove(selected);
                }
                else // Selecting
                {
                    if (!SkillTreeModelHelpers.HasUnmetDependencies(GetAllSelectedSkill(), CurrentTab.SkillSquareCursorIsOver(CursorPosition)))
                        NewlySelectedSkills.Add(selected);
                }
            }
        }

        public override void UpdateFromNewData (IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
            m_engine = engine;
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
            }
        }
    }
}
