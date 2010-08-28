using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using System;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreeTab
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Point DefaultCurorPosition { get; set; }
        public List<SkillSquare> SkillSquares { get; set; }
        public char[,] CharArray;

        public const int ExplainPopupHeight = 18;
        public const int ExplainPopupWidth = 26;
        
        private DialogColorHelper m_dialogHelper;
        private Dictionary<Point, SkillSquare> m_squareLookup;

        // Using TCODColor.color involves a P/Invoke, so for high trafic calls we cache.
        private readonly TCODColor m_black = TCODColor.black;
        private readonly TCODColor m_darkBlue = TCODColor.darkBlue;
        private readonly TCODColor m_lightBlue = TCODColor.lightBlue;
        private readonly TCODColor m_lightestBlue = TCODColor.lightestBlue;
        private readonly TCODColor m_blue = TCODColor.blue;

        internal SkillTreeTab(string resourceToRead)
        {
            m_dialogHelper = new DialogColorHelper();
            SkillSquares = new List<SkillSquare>();

            SkillTreeTabXMLHelper xmlHelper = new SkillTreeTabXMLHelper(this);
            xmlHelper.LoadMappings(resourceToRead);

            m_squareLookup = new Dictionary<Point, SkillSquare>(PointEqualityComparer.Instance);
            foreach(SkillSquare s in SkillSquares)
            {
                foreach (Point p in s.PointsSquareCovers)
                {
                    m_squareLookup.Add(p, s);
                }
            }
        }

        public void Draw(TCODConsole console, IGameEngine engine, List<ISkill> selectedSkillList, List<ISkill> newlySelectedSkillList, Point cursorPosition)
        {
            SkillSquare cursorSkillSquare = null;
            ISkill cursorOverSkill = null;
            if (m_squareLookup.ContainsKey(cursorPosition))
            {
                cursorSkillSquare = m_squareLookup[cursorPosition];
                cursorOverSkill = cursorSkillSquare.GetSkill(engine);
            }
            int selectedSkillCost = newlySelectedSkillList.Sum(x => x.Cost);

            int upperLeftX = cursorPosition.X - ((SkillTreePainter.SkillTreeWidth - 1) / 2);
            int upperLeftY = cursorPosition.Y - ((SkillTreePainter.SkillTreeHeight - 1) / 2);

            int lowerRightX = cursorPosition.X - ((SkillTreePainter.SkillTreeWidth - 1) / 2);
            int lowerRightY = cursorPosition.Y - ((SkillTreePainter.SkillTreeHeight - 1) / 2);

            int arrayWidth = CharArray.GetLength(0);
            int arrayHeight = CharArray.GetLength(1);

            for (int i = SkillTreePainter.UpperLeft + 1; i < SkillTreePainter.SkillTreeWidth + SkillTreePainter.UpperLeft - 1; ++i)
            {
                for (int j = SkillTreePainter.UpperLeft + 1; j < SkillTreePainter.SkillTreeHeight + SkillTreePainter.UpperLeft -1; ++j)
                {
                    int gridX;
                    int gridY;
                    ConvertDrawToGridCoord(i, j, cursorPosition, out gridX, out gridY);
                    if (gridX >= 0 && gridY >= 0 && gridX < arrayWidth && gridY < arrayHeight)
                    {
                        //CalculateBackgroundColorForSkill
                        // We're painting something that shows up on our "grid"
                        TCODColor background = CalculateBackgroundColorForSkill(new Point(gridX, gridY), cursorPosition, selectedSkillList, newlySelectedSkillList, engine.Player.SkillPoints, selectedSkillCost, cursorOverSkill);
                        console.putCharEx(i, j, CharArray[gridX, gridY], UIHelper.ForegroundColor, background);
                    }
                    else
                    {
                        // We're not painting something on our grid, black it out
                        console.setCharBackground(i, j, m_black);
                    }
                }
            }

            if (cursorSkillSquare != null)
                DrawSkillPopup(console, selectedSkillList, engine.Player.SkillPoints - selectedSkillCost, cursorSkillSquare, cursorOverSkill, cursorPosition);
        }

        // Out params are normally not preferred, but this gets called for every square painted, so we don't want to have to alloc Points
        private void ConvertDrawToGridCoord(int drawX, int drawY, Point cursorPosition, out int gridX, out int gridY)
        {
            gridX = cursorPosition.X - SkillTreePainter.SkillTreeCursorPosition.X + drawX;
            gridY = cursorPosition.Y - SkillTreePainter.SkillTreeCursorPosition.Y + drawY;
        }

        private Point ConvertGridToDrawCoord(Point gridPoint, Point cursorPosition)
        {
            return SkillTreePainter.SkillTreeCursorPosition - cursorPosition + gridPoint;
        }

        private TCODColor CalculateBackgroundColorForSkill(Point gridPosition, Point cursorPosition, List<ISkill> selectedSkillList, List<ISkill> newlySelectedSkillList, int playerSkillPoints, int selectedSkillCost, ISkill cursorOverSkill)
        {
            if (m_squareLookup.ContainsKey(gridPosition))
            {
                SkillSquare skillSquareBeingPained = m_squareLookup[gridPosition];
                bool cursorInMySquare = skillSquareBeingPained.IsInSquare(cursorPosition);

                bool selectedMySquare = SkillTreeModelHelpers.IsSkillSelected(selectedSkillList, skillSquareBeingPained.SkillName);

                if (cursorInMySquare)
                {
                    if (selectedMySquare)
                    {
                        return m_blue;
                    }
                    else
                    {
                        bool hasEnoughSkillPointsToSelectThisAsWell = selectedSkillCost + cursorOverSkill.Cost <= playerSkillPoints;
                        bool hasDependenciesMet = !SkillTreeModelHelpers.HasUnmetDependencies(selectedSkillList, skillSquareBeingPained);
                        if (hasDependenciesMet && hasEnoughSkillPointsToSelectThisAsWell)
                            return m_lightBlue;
                        else
                            return m_lightestBlue;
                    }
                }
                else
                {
                    if (selectedMySquare)
                        return m_blue;
                    else
                        return m_black;
                }
            }
            else
            {
                return m_black;
            } 
        }

        private void DrawSkillPopup(TCODConsole console, List<ISkill> selectedSkillList, int skillPointsLeft, SkillSquare cursorSkillSquare, ISkill cursorOverSkill, Point cursorPosition)
        {
            Point drawUpperLeft = ConvertGridToDrawCoord(cursorSkillSquare.UpperRight + new Point(1, 1), cursorPosition);
            
            int numberOfDependencies = cursorSkillSquare.DependentSkills.Count();
            int dialogHeight = ExplainPopupHeight;
            if (numberOfDependencies > 0)
            {
                int linesOfDependencies = numberOfDependencies;
                foreach (string dependentSkillName in cursorSkillSquare.DependentSkills)
                    linesOfDependencies += console.getHeightRect(drawUpperLeft.X + 2 + 3, 0, 20, 2, dependentSkillName);
                
                dialogHeight += 2 + linesOfDependencies;
                drawUpperLeft += new Point(0, 2 + linesOfDependencies);
            }

            string title = cursorOverSkill.Name;
            if (title.Length > 22)
                title = title.Substring(0, 22);

            console.printFrame(drawUpperLeft.X, drawUpperLeft.Y - dialogHeight, ExplainPopupWidth, dialogHeight, true, TCODBackgroundFlag.Set, title);

            int textX = drawUpperLeft.X + 2;
            int textY = drawUpperLeft.Y - dialogHeight + 2;

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

                    console.printRect(textX + 3, textY, 20, 2, dependentSkillName);
                    textY++;
                }
                m_dialogHelper.ResetColors(console);
            }
            textY++;

            console.printRectEx(textX, textY, ExplainPopupWidth - 4, ExplainPopupHeight - 6, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, cursorOverSkill.Description);
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
    }
}
