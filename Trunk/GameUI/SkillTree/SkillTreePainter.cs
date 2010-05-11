using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreePainter : MapPainterBase
    {
        private class SkillSquare
        {
            public Point UpperLeft;
            public Point LowerRight;
            public string SkillName;

            public SkillSquare(Point upperLeft, Point lowerRight)
            {
                UpperLeft = upperLeft;
                LowerRight = lowerRight;
                SkillName = null;
                Skill = null;
                m_dependentSkills = new List<string>();
            }

            public bool IsInSquare(Point p)
            {
                return p.X > UpperLeft.X && p.X < LowerRight.X && p.Y > UpperLeft.Y && p.Y < LowerRight.Y;
            }

            // We do this to cache the skill, since on init time we don't have the IGameEngine to resolve
            private ISkill Skill;
            public ISkill GetSkill(IGameEngine engine)
            {
                if (Skill == null)
                    Skill = engine.GetSkillFromName(SkillName);
                return Skill;
            }

            private List<string> m_dependentSkills;
            public IList<string> DependentSkills
            {
                get
                {
                    return m_dependentSkills;
                }
            }

            public void AddDependency(string dependentSkillName)
            {
                m_dependentSkills.Add(dependentSkillName);
            }

            public Point UpperRight
            {
                get
                {
                    return UpperLeft + new Point(LowerRight.X - UpperLeft.X, 0);
                }
            }
        }

        private int m_width;
        private int m_height;
        private Point m_defaultCurorPosition;
        private char[,] m_array;
        private TCODConsole m_offscreenConsole;
        private List<SkillSquare> m_skillSquares;
        private DialogColorHelper m_dialogHelper;

        internal List<ISkill> NewlySelectedSkills { get; private set; }

        private IGameEngine m_engine;

        private const int UpperLeft = 5;
        private const int ScreenWidth = 70;
        private const int ScreenHeight = 50;
        private static Point SkillTreeScreenCenter = new Point(((ScreenWidth - 1) / 2), ((ScreenHeight - 1) / 2));

        private const char UpperLeftSymbol = '1';
        private const char UpperRightSymbol = '2';
        private const char LowerLeftSymbol = '3';
        private const char LowerRightSymbol = '4';

        const int ExplainPopupHeight = 18;
        const int ExplainPopupWidth = 26;

        internal SkillTreePainter()
        {
            m_enabled = false;
            CursorPosition = Point.Invalid;
            NewlySelectedSkills = new List<ISkill>();
            m_skillSquares = new List<SkillSquare>();
            m_dialogHelper = new DialogColorHelper();

            ReadSkillTreeFile();
        }

        // TODO - Rewrite this and data file to be XML.
        private void ReadSkillTreeFile()
        {
            string fileName = Path.Combine(Path.Combine(AssemblyDirectory.CurrentAssemblyDirectory, "Resources"), "SkillTree.dat");
            using(StreamReader s = new StreamReader(fileName))
            {
                string [] sizeLine = s.ReadLine().Split(' ');
                m_width = int.Parse(sizeLine[0]);
                m_height = int.Parse(sizeLine[1]);
                m_defaultCurorPosition = new Point(int.Parse(sizeLine[2]), int.Parse(sizeLine[3]));
                m_array = new char[m_width, m_height];
                m_offscreenConsole = new TCODConsole(m_width + ExplainPopupWidth + 1, m_height + ExplainPopupHeight + 1);
                List<Point> m_cornersUnmatched = new List<Point>();
                for(int j = 0 ; j < m_height ; ++j)
                {
                    string line = s.ReadLine();
                    for(int i = 0 ; i < m_width ; ++i)
                    {
                        m_array[i, j] = line[i];
                        if (line[i] == UpperLeftSymbol)
                            m_cornersUnmatched.Add(new Point(i, j));
                        if (line[i] == LowerRightSymbol)
                        {
                            Point searchPoint = new Point(i, j) - new Point(1, 1);
                            while (true)
                            {
                                if (searchPoint.X < 0 || searchPoint.Y < 0)
                                    break;

                                if (m_array[searchPoint.X, searchPoint.Y] == UpperLeftSymbol)
                                {
                                    m_cornersUnmatched.Remove(searchPoint);
                                    m_skillSquares.Add(new SkillSquare(searchPoint, new Point(i, j)));
                                    break;
                                }

                                searchPoint -= new Point(1, 1);
                            }
                        }
                    }
                }

                if(m_cornersUnmatched.Count > 0)
                    throw new System.InvalidOperationException("Unmatched corners in SkillTree data");

                while (!s.EndOfStream)
                {
                    string line = s.ReadLine();
                    string[] positionNameParts = line.Split(new char[] {' '}, 3);
                    if (positionNameParts.Length == 3)
                    {
                        string name = StripDependencyFromNameString(positionNameParts[2]);
                        Point upperLeft = new Point(int.Parse(positionNameParts[0]), int.Parse(positionNameParts[1]));

                        if (m_skillSquares.Exists(x => x.UpperLeft == upperLeft))
                        {
                            SkillSquare currentSkillSquare = m_skillSquares.Find(x => x.UpperLeft == upperLeft);
                            currentSkillSquare.SkillName = name;

                            int dependencyStart = line.IndexOf('{');
                            if(dependencyStart != -1)
                            {
                                string dependencyString = line.Remove(0, dependencyStart + 1); // +1 to get {
                                var v = dependencyString.Split(',');
                                foreach(string dependentSkillName in dependencyString.Split(','))
                                {
                                    if(m_skillSquares.Exists(x => x.SkillName == dependentSkillName))
                                        currentSkillSquare.AddDependency(dependentSkillName);
                                    else
                                        throw new InvalidOperationException(string.Format("Unable to find dependency listed. \"{0}\" - \"{1}\"", currentSkillSquare.SkillName, dependentSkillName));
                                }
                            }
                        }
                        else
                            throw new System.InvalidOperationException("Unable to find square mentioned in listing below");
                    }
                }
            }
        }

        private string StripDependencyFromNameString(string s)
        {
            int dependencyStart = s.IndexOf('{'); // Remove dependency info from name
            return dependencyStart == -1 ? s : s.Remove(dependencyStart);
        }

        private char ConvertFileCharToPrintChar(char c)
        {
            switch(c)
            {
                case UpperLeftSymbol:
                    return (char)TCODSpecialCharacter.NW;
                case UpperRightSymbol:
                    return (char)TCODSpecialCharacter.NE;
                case LowerLeftSymbol:
                    return (char)TCODSpecialCharacter.SW;
                case LowerRightSymbol:
                    return (char)TCODSpecialCharacter.SE;
                case '.':
                    return ' ';
                case '"':
                    return (char)TCODSpecialCharacter.VertLine;
                case '\'':
                    return (char)TCODSpecialCharacter.HorzLine;
                default:
                    return c;
            }
        }

        public override void DrawNewFrame (TCODConsole screen)
        {
            if (Enabled)
            {
                DrawOffSceenConsole();
                int lowX = CursorPosition.X - (ScreenWidth / 2);
                int lowY = CursorPosition.Y - (ScreenHeight / 2);
                screen.printFrame(UpperLeft, UpperLeft, ScreenWidth, ScreenHeight, true, TCODBackgroundFlag.Set, "Skill Tree");
                TCODConsole.blit(m_offscreenConsole, lowX, lowY, ScreenWidth - 2, ScreenHeight - 2, screen, UpperLeft + 1, UpperLeft + 1);

                screen.setCharBackground(SkillTreeScreenCenter.X + UpperLeft + 2, SkillTreeScreenCenter.Y + UpperLeft + 2, TCODColor.darkGrey);

                // screen.print(50, 50, m_cursorPosition.ToString());
            }
        }

        private ISkill SkillCursorIsOver
        {
            get
            {
                foreach (SkillSquare skillSquare in m_skillSquares)
                {
                    if (skillSquare.IsInSquare(m_cursorPosition))
                    {
                        return skillSquare.GetSkill(m_engine);
                    }
                }
                return null;
            }
        }

        private SkillSquare SkillSquareCursorIsOver
        {
            get
            {
                foreach (SkillSquare skillSquare in m_skillSquares)
                {
                    if (skillSquare.IsInSquare(m_cursorPosition))
                    {
                        return skillSquare;
                    }
                }
                return null;
            }
        }

        // TODO - This needs to only select squares that weren't part of the character list, and only allow
        // unselection of those
        internal void SelectSquare()
        {
            ISkill selected = SkillCursorIsOver;
            if (selected != null)
            {
                // Don't allow skills to be deselected once chosen.
                if (m_engine.Player.Skills.Contains(selected))
                    return;

                if (NewlySelectedSkills.Contains(selected)) // Deselecting
                {
                    bool somebodyHasDependencyOnLeavingSkill = false;
                    foreach(ISkill s in GetAllSelectedSkill())
                    {
                        if (HasDependencyOn(s.Name, selected.Name))
                            somebodyHasDependencyOnLeavingSkill = true;
                    }
                    if (!somebodyHasDependencyOnLeavingSkill)
                        NewlySelectedSkills.Remove(selected);
                }
                else // Selecting
                {
                     if (!HasUnmetDependencies(SkillSquareCursorIsOver))
                        NewlySelectedSkills.Add(selected);
                }
            }
        }

        private bool HasDependencyOn(string parentSkill, string possibleDependency)
        {
            return m_skillSquares.Find(x => x.SkillName == parentSkill).DependentSkills.Contains(possibleDependency);
        }

        private bool HasUnmetDependencies(SkillSquare skill)
        {
            foreach(string dependencyName in skill.DependentSkills)
            {
                if (!IsSkillSelected(dependencyName))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsSkillSelected(string skillName)
        {
            return GetAllSelectedSkill().ToList().Exists(x => x.Name == skillName);
        }

        private IEnumerable<ISkill> GetAllSelectedSkill()
        {
            List<ISkill> list = new List<ISkill>();
            list.AddRange(NewlySelectedSkills);
            list.AddRange(m_engine.Player.Skills);
            return list;
        }

        private void DrawOffSceenConsole()
        {
            m_offscreenConsole.clear();

            SkillSquare cursorSkillSquare = m_skillSquares.Find(x => x.IsInSquare(m_cursorPosition));
            ISkill cursorOverSkill = (cursorSkillSquare != null) ? cursorSkillSquare.GetSkill(m_engine) : null;

            for (int i = 0 ; i < m_width ; ++i)
            {
                for (int j = 0 ; j < m_height ; ++j)
                {
                    m_offscreenConsole.putChar(i, j, ConvertFileCharToPrintChar(m_array[i,j]), TCODBackgroundFlag.Set);

                    SkillSquare skillSquareBeingPained = m_skillSquares.Find(x => x.IsInSquare(new Point(i, j)));
                    if (skillSquareBeingPained != null)
                    {
                        bool cursorInMySquare = skillSquareBeingPained.IsInSquare(m_cursorPosition);

                        bool selectedMySquare = IsSkillSelected(skillSquareBeingPained.SkillName);

                        TCODColor background;
                        if (cursorInMySquare)
                        {
                            if (selectedMySquare)
                            {
                                background = TCODColor.darkBlue;
                            }
                            else
                            {
                                if(!HasUnmetDependencies(skillSquareBeingPained))
                                    background = TCODColor.lightBlue;
                                else
                                    background = TCODColor.lightestBlue;
                            }
                        }
                        else
                        {
                            if (selectedMySquare)
                                background = TCODColor.blue;
                            else
                                background = TCODColor.black;
                        }

                        if(background != TCODColor.black)
                            m_offscreenConsole.setCharBackground(i, j, background);
                    }
                }
            }

            if (cursorSkillSquare != null)
            {
                DrawSkillPopup(cursorSkillSquare, cursorOverSkill);
            }
        }

        private void DrawSkillPopup(SkillSquare cursorSkillSquare, ISkill cursorOverSkill)
        {
            Point explainationBoxLowerLeft = cursorSkillSquare.UpperRight + new Point(1, 0);
            int numberOfDependencies = cursorSkillSquare.DependentSkills.Count();
            int dialogHeight = ExplainPopupHeight;
            if(numberOfDependencies > 0)
            {
                dialogHeight += 1 + numberOfDependencies;
                explainationBoxLowerLeft += new Point(0, numberOfDependencies + 1);
            }

            m_offscreenConsole.printFrame(explainationBoxLowerLeft.X, explainationBoxLowerLeft.Y - dialogHeight,
                                          ExplainPopupWidth, dialogHeight, true, TCODBackgroundFlag.Set, cursorOverSkill.Name);

            int textX = explainationBoxLowerLeft.X + 2;
            int textY = explainationBoxLowerLeft.Y - dialogHeight + 2;
            if(cursorOverSkill.NewSpell)
            {
                m_offscreenConsole.print(textX, textY, "New Spell");
                textY++;
            }
            m_offscreenConsole.print(textX, textY, string.Format("School: {0}", cursorOverSkill.School));
            textY++;
            m_offscreenConsole.print(textX, textY, string.Format("Skill Point Cost: {0}", cursorOverSkill.Cost));
            textY += 2;

            if (numberOfDependencies > 0)
            {
                m_offscreenConsole.print(textX, textY, "Dependencies:");
                textY++;
                m_dialogHelper.SaveColors(m_offscreenConsole);
                foreach(string dependentSkillName in cursorSkillSquare.DependentSkills)
                {
                    if (IsSkillSelected(dependentSkillName))
                        m_dialogHelper.SetColors(m_offscreenConsole, false, true);
                    else
                        m_dialogHelper.SetColors(m_offscreenConsole, false, false);
                    m_offscreenConsole.print(textX, textY, "   " + dependentSkillName);
                    textY++;
                }
                m_dialogHelper.ResetColors(m_offscreenConsole);
            }
            textY++;

            m_offscreenConsole.printRectEx(textX, textY, ExplainPopupWidth - 4, ExplainPopupHeight - 7,
                                           TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, cursorOverSkill.Description);
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
                CursorPosition = m_defaultCurorPosition;
                NewlySelectedSkills.Clear();
            }
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
            }
        }
    }
}
