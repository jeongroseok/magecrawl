using System.Collections.Generic;
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
        public char[,] CharArray;

        public const int ExplainPopupHeight = 18;
        public const int ExplainPopupWidth = 26;
        
        private DialogColorHelper m_dialogHelper;

        private const char UpperLeftSymbol = '1';
        private const char UpperRightSymbol = '2';
        private const char LowerLeftSymbol = '3';
        private const char LowerRightSymbol = '4';

        internal SkillTreeTab(string resourceToRead)
        {
            m_dialogHelper = new DialogColorHelper();
            SkillSquares = new List<SkillSquare>();

            SkillTreeTabXMLHelper xmlHelper = new SkillTreeTabXMLHelper(this);
            xmlHelper.LoadMappings(resourceToRead);
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
                    console.putChar(drawPosition.X, drawPosition.Y, CharArray[i, j], TCODBackgroundFlag.Set);

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
                            console.setCharBackground(drawPosition.X, drawPosition.Y, background);
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

            console.printFrame(explainationBoxLowerLeft.X, explainationBoxLowerLeft.Y - dialogHeight, ExplainPopupWidth, dialogHeight, true, TCODBackgroundFlag.Set, cursorOverSkill.Name);

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
