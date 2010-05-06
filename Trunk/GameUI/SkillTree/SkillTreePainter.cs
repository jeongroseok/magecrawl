using System.Collections.Generic;
using System.Linq;
using System.IO;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreePainter : MapPainterBase
    {
        private class SkillSquare
        {
            public SkillSquare(Point upperLeft, Point lowerRight)
            {
                UpperLeft = upperLeft;
                LowerRight = lowerRight;
                SkillName = null;
                Skill = null;
            }

            public bool IsInSquare(Point p)
            {
                return p.X > UpperLeft.X && p.X < LowerRight.X && p.Y > UpperLeft.Y && p.Y < LowerRight.Y;
            }

            public Point UpperLeft;
            public Point LowerRight;
            public string SkillName;

            // We do this to cache the skill, since on init time we don't have the IGameEngine to resolve
            private ISkill Skill;
            public ISkill GetSkill(IGameEngine engine)
            {
                if (Skill == null)
                    Skill = engine.GetSkill(SkillName);
                return Skill;
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
        private List<SkillSquare> m_skillSquares = new List<SkillSquare>();
        private List<ISkill> m_newlySelectedSkills = new List<ISkill>();

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
            Enabled = false;
            CursorPosition = Point.Invalid;
            m_newlySelectedSkills = new List<ISkill>();

            ReadSkillTreeFile();
        }

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
                    string[] parts = line.Split(new char[] {' '}, 3);
                    if (parts.Length == 3)
                    {
                        Point upperLeft = new Point(int.Parse(parts[0]), int.Parse(parts[1]));

                        if (m_skillSquares.Exists(x => x.UpperLeft == upperLeft))
                            m_skillSquares.Find(x => x.UpperLeft == upperLeft).SkillName = parts[2];
                        else
                            throw new System.InvalidOperationException("Unable to find square mentioned in listing below");
                    }
                }
            }
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

        internal ISkill Selected
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

        internal void SelectSquare()
        {
            ISkill selected = Selected;
            if (selected != null)
            {
                if (m_newlySelectedSkills.Contains(selected))
                    m_newlySelectedSkills.Remove(selected);
                else
                    m_newlySelectedSkills.Add(selected);
            }
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
                        bool selectedMySquare = m_newlySelectedSkills.Exists(x => x.Name == skillSquareBeingPained.SkillName);

                        TCODColor background;

                        if (cursorInMySquare)
                        {
                            if (selectedMySquare)
                                background = TCODColor.darkBlue;
                            else
                                background = TCODColor.lightBlue;
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
                Point explainationBoxLowerLeft = cursorSkillSquare.UpperRight + new Point(1, 0);
                m_offscreenConsole.printFrame(explainationBoxLowerLeft.X, explainationBoxLowerLeft.Y - ExplainPopupHeight, ExplainPopupWidth, ExplainPopupHeight, true, TCODBackgroundFlag.Set, cursorOverSkill.Name);

                int textX = explainationBoxLowerLeft.X + 2;
                int textY = explainationBoxLowerLeft.Y - ExplainPopupHeight + 2;
                if(cursorOverSkill.NewSpell)
                {
                    m_offscreenConsole.print(textX, textY, "New Spell");
                    textY++;
                }
                m_offscreenConsole.print(textX, textY, string.Format("School: {0}", cursorOverSkill.School));
                textY++;
                m_offscreenConsole.print(textX, textY, string.Format("Skill Point Cost: {0}", cursorOverSkill.Cost));
                textY += 2;
                m_offscreenConsole.printRectEx(textX, textY, ExplainPopupWidth - 4, ExplainPopupHeight - 7, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, cursorOverSkill.Description);
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
                CursorPosition = m_defaultCurorPosition;
                m_newlySelectedSkills.Clear();
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
