using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreeTab
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Point DefaultCurorPosition { get; set; }
        public List<SkillSquare> SkillSquares { get; set; }

        public const int ExplainPopupHeight = 18;
        public const int ExplainPopupWidth = 26;
        
        private DialogColorHelper m_dialogHelper;
        private char[,] m_charArray;

        private const char UpperLeftSymbol = '1';
        private const char UpperRightSymbol = '2';
        private const char LowerLeftSymbol = '3';
        private const char LowerRightSymbol = '4';

        internal SkillTreeTab(string resourceToRead)
        {
            m_dialogHelper = new DialogColorHelper();
            SkillSquares = new List<SkillSquare>();
            ReadSkillTreeFile(resourceToRead);
        }

        public void Draw(TCODConsole console, IGameEngine engine, List<ISkill> selectedSkillList, List<ISkill> newlySelectedSkillList, Point cursorPosition)
        {
            console.clear();

            SkillSquare cursorSkillSquare = SkillSquares.Find(x => x.IsInSquare(cursorPosition));
            ISkill cursorOverSkill = (cursorSkillSquare != null) ? cursorSkillSquare.GetSkill(engine) : null;
            int selectedSkillCost = newlySelectedSkillList.Sum(x => x.Cost);
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Point mapPosition = new Point(i, j);
                    Point drawPosition = mapPosition + new Point(0, ExplainPopupHeight);
                    console.putChar(drawPosition.X, drawPosition.Y, ConvertFileCharToPrintChar(m_charArray[i, j]), TCODBackgroundFlag.Set);

                    SkillSquare skillSquareBeingPained = SkillSquares.Find(x => x.IsInSquare(mapPosition));
                    if (skillSquareBeingPained != null)
                    {
                        bool cursorInMySquare = skillSquareBeingPained.IsInSquare(cursorPosition);

                        bool selectedMySquare = SkillTreeModelHelpers.IsSkillSelected(selectedSkillList, skillSquareBeingPained.SkillName);

                        TCODColor background;
                        if (cursorInMySquare)
                        {
                            if (selectedMySquare)
                            {
                                background = TCODColor.darkBlue;
                            }
                            else
                            {
                                bool hasEnoughSkillPointsToSelectThisAsWell = selectedSkillCost + cursorOverSkill.Cost <= engine.Player.SkillPoints;
                                bool hasDependenciesMet = !SkillTreeModelHelpers.HasUnmetDependencies(selectedSkillList, skillSquareBeingPained);
                                if (hasDependenciesMet && hasEnoughSkillPointsToSelectThisAsWell)
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

                        if (background != TCODColor.black)
                            console.setCharBackground(drawPosition.X, drawPosition.Y , background);
                    }
                }
            }

            if (cursorSkillSquare != null)
                DrawSkillPopup(console, selectedSkillList, engine.Player.SkillPoints - selectedSkillCost, cursorSkillSquare, cursorOverSkill);
        }

        private void DrawSkillPopup(TCODConsole console, List<ISkill> selectedSkillList, int skillPointsLeft, SkillSquare cursorSkillSquare, ISkill cursorOverSkill)
        {
            Point explainationBoxLowerLeft = cursorSkillSquare.UpperRight + new Point(1, 0) + new Point(0, ExplainPopupHeight);
            int numberOfDependencies = cursorSkillSquare.DependentSkills.Count();
            int dialogHeight = ExplainPopupHeight;
            if (numberOfDependencies > 0)
            {
                dialogHeight += 2 + numberOfDependencies;
                explainationBoxLowerLeft += new Point(0, numberOfDependencies + 2);
            }

            console.printFrame(explainationBoxLowerLeft.X, explainationBoxLowerLeft.Y - dialogHeight,
                                          ExplainPopupWidth, dialogHeight, true, TCODBackgroundFlag.Set, cursorOverSkill.Name);

            int textX = explainationBoxLowerLeft.X + 2;
            int textY = explainationBoxLowerLeft.Y - dialogHeight + 2;
            
            // If we can't afford it and it isn't already selected, show the cost in red
            if (!selectedSkillList.Contains(cursorOverSkill) && cursorOverSkill.Cost > skillPointsLeft)
            {
                m_dialogHelper.SaveColors(console);
                console.setForegroundColor(TCODColor.red);
                console.print(textX, textY, string.Format("Skill Point Cost: {0}", cursorOverSkill.Cost));
                m_dialogHelper.ResetColors(console);
            }
            else
            {
                console.print(textX, textY, string.Format("Skill Point Cost: {0}", cursorOverSkill.Cost));
            }
            textY++;

            if (numberOfDependencies > 0)
            {
                textY++;
                console.print(textX, textY, "Dependencies:");
                textY++;
                m_dialogHelper.SaveColors(console);
                foreach (string dependentSkillName in cursorSkillSquare.DependentSkills)
                {
                    if (SkillTreeModelHelpers.IsSkillSelected(selectedSkillList, dependentSkillName))
                        m_dialogHelper.SetColors(console, false, true);
                    else
                        m_dialogHelper.SetColors(console, false, false);
                    console.print(textX, textY, "   " + dependentSkillName);
                    textY++;
                }
                m_dialogHelper.ResetColors(console);
            }
            textY++;

            console.printRectEx(textX, textY, ExplainPopupWidth - 4, ExplainPopupHeight - 6,
                                           TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, cursorOverSkill.Description);
        }

        public ISkill SkillCursorIsOver(IGameEngine engine, Point cursorPosition)
        {
            foreach (SkillSquare skillSquare in SkillSquares)
            {
                if (skillSquare.IsInSquare(cursorPosition))
                    return skillSquare.GetSkill(engine);
            }
            return null;
        }

        public SkillSquare SkillSquareCursorIsOver(Point cursorPosition)
        {
            foreach (SkillSquare skillSquare in SkillSquares)
            {
                if (skillSquare.IsInSquare(cursorPosition))
                    return skillSquare;
            }
            return null;
        }

        private char ConvertFileCharToPrintChar(char c)
        {
            switch (c)
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

        // TODO - Rewrite this and data file to be XML.
        private void ReadSkillTreeFile(string resourceToRead)
        {
            string fileName = Path.Combine(Path.Combine(Path.Combine(AssemblyDirectory.CurrentAssemblyDirectory, "Resources"), "Skill Tree"), resourceToRead);
            using (StreamReader s = new StreamReader(fileName))
            {
                string[] sizeLine = s.ReadLine().Split(' ');
                Width = int.Parse(sizeLine[0]);
                Height = int.Parse(sizeLine[1]);
                DefaultCurorPosition = new Point(int.Parse(sizeLine[2]), int.Parse(sizeLine[3]));
                m_charArray = new char[Width, Height];
                List<Point> m_cornersUnmatched = new List<Point>();
                for (int j = 0; j < Height; ++j)
                {
                    string line = s.ReadLine();
                    for (int i = 0; i < Width; ++i)
                    {
                        m_charArray[i, j] = line[i];
                        if (line[i] == UpperLeftSymbol)
                            m_cornersUnmatched.Add(new Point(i, j));
                        if (line[i] == LowerRightSymbol)
                        {
                            Point searchPoint = new Point(i, j) - new Point(1, 1);
                            while (true)
                            {
                                if (searchPoint.X < 0 || searchPoint.Y < 0)
                                    break;

                                if (m_charArray[searchPoint.X, searchPoint.Y] == UpperLeftSymbol)
                                {
                                    m_cornersUnmatched.Remove(searchPoint);
                                    SkillSquares.Add(new SkillSquare(searchPoint, new Point(i, j)));
                                    break;
                                }

                                searchPoint -= new Point(1, 1);
                            }
                        }
                    }
                }

                if (m_cornersUnmatched.Count > 0)
                    throw new System.InvalidOperationException("Unmatched corners in SkillTree data");

                while (!s.EndOfStream)
                {
                    string line = s.ReadLine();
                    string[] positionNameParts = line.Split(new char[] { ' ' }, 3);
                    if (positionNameParts.Length == 3)
                    {
                        string name = StripDependencyFromNameString(positionNameParts[2]);
                        Point upperLeft = new Point(int.Parse(positionNameParts[0]), int.Parse(positionNameParts[1]));

                        if (SkillSquares.Exists(x => x.UpperLeft == upperLeft))
                        {
                            SkillSquare currentSkillSquare = SkillSquares.Find(x => x.UpperLeft == upperLeft);
                            currentSkillSquare.SkillName = name;

                            int dependencyStart = line.IndexOf('{');
                            if (dependencyStart != -1)
                            {
                                string dependencyString = line.Remove(0, dependencyStart + 1); // +1 to get {
                                foreach (string dependentSkillName in dependencyString.Split(','))
                                {
                                    if (SkillSquares.Exists(x => x.SkillName == dependentSkillName))
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
    }
}
