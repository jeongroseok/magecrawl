using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using libtcod;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreeTabXMLHelper
    {
        private SkillTreeTab m_tabLoading;

        // Square is 7x7, 4 between columns, 2 between rows
        private const int SquareSize = 7;
        private const int OffsetToCenterOfSize = 3;
        private const int GapBetweenColumns = 4;
        private const int GapBetweenRows = 2;
        private const int ColumnOffSetPer = SquareSize + GapBetweenColumns;
        private const int RowOffSetPer = SquareSize + GapBetweenRows;

        private int m_maxRow;
        private int m_largestColumn;
        private int m_largestRow;

        private List<Pair<Point, string>> m_iconTextLines;

        internal SkillTreeTabXMLHelper(SkillTreeTab tabLoading)
        {
            m_iconTextLines = new List<Pair<Point, string>>();
            
            m_largestColumn = int.MinValue;
            m_largestRow = int.MinValue;

            m_tabLoading = tabLoading;            
        }

        private Point CalculatePosition(int column, int row)
        {
            if (row > m_maxRow)
                throw new InvalidOperationException("SkillTreeTabXMLHelper - Row larger than max row?");

            // Row count from the bottom to make adding new elements on top of tree easier
            // | (0,1)    (1,1)
            // | (0,0)    (1,0)
            // ------------------
            return new Point(column * ColumnOffSetPer, (m_maxRow - row) * RowOffSetPer);
        }

        private void HandleNewPoint(int column, int row)
        {
            m_largestColumn = Math.Max(m_largestColumn, column);
            m_largestRow = Math.Max(m_largestRow, row);
        }

        public void LoadMappings(string resourceToRead)
        {
            XMLResourceReaderBase.ParseFile(Path.Combine("Skill Tree", resourceToRead), ReadFileCallback);

            int width = m_largestColumn * ColumnOffSetPer + SquareSize;
            int height = m_largestRow * RowOffSetPer + SquareSize;
            m_tabLoading.CharArray = new char[width, height];
            m_tabLoading.Width = width;
            m_tabLoading.Height = height;

            DrawCharArray();
        }

        private void SetChar(Point p, char c)
        {
            m_tabLoading.CharArray[p.X, p.Y] = c;
        }

        private void DrawCharArray()
        {
            // Draw squares
            foreach (SkillSquare s in m_tabLoading.SkillSquares)
            {
                SetChar(s.UpperLeft, (char)TCODSpecialCharacter.NW);
                SetChar(s.UpperLeft + new Point(6,0), (char)TCODSpecialCharacter.NE);
                SetChar(s.UpperLeft + new Point(0,6), (char)TCODSpecialCharacter.SW);
                SetChar(s.UpperLeft + new Point(6,6), (char)TCODSpecialCharacter.SE);
                for (int i = 1 ; i < SquareSize - 1 ; i++)
                {
                    SetChar(s.UpperLeft + new Point(i, 0), (char)TCODSpecialCharacter.HorzLine);
                    SetChar(s.UpperLeft + new Point(i, SquareSize - 1), (char)TCODSpecialCharacter.HorzLine);

                    SetChar(s.UpperLeft + new Point(0, i), (char)TCODSpecialCharacter.VertLine);
                    SetChar(s.UpperLeft + new Point(SquareSize - 1, i), (char)TCODSpecialCharacter.VertLine);
                }
            }

            // Draw text
            foreach (Pair<Point, string> textBlob in m_iconTextLines)
            {
                int i = 0;
                foreach (char c in textBlob.Second)
                {
                    SetChar(textBlob.First + new Point(i, 0), c);
                    i++;
                }
            }

            // Draw dependency lines
            foreach (SkillSquare skill in m_tabLoading.SkillSquares)
            {
                foreach (string dependentSkillName in skill.DependentSkills)
                {
                    SkillSquare dependentSkill = m_tabLoading.SkillSquares.Where(x => x.SkillName == dependentSkillName).First();
                    DrawDependencyLine(skill, dependentSkill);
                }
            }
        }

        // Drawing each dependency line is a special case, 
        // so drawing between x and an adjacent row/column is a case
        //  5  1  4
        //  3  x  2
        private void DrawDependencyLine(SkillSquare current, SkillSquare dependent)
        {
            // Flip the polarity of the rows sine we're counting bottom up for those (backwards)
            int rowDifference = -1 * ((current.UpperLeft.Y - dependent.UpperLeft.Y) / RowOffSetPer);
            int columnDifference = (current.UpperLeft.X - dependent.UpperLeft.X) / ColumnOffSetPer;

            if (rowDifference >= 1 && columnDifference == 0)
            {
                // 1
                for (int i = 1; i <= GapBetweenRows + ((rowDifference - 1) * (GapBetweenRows + SquareSize)); ++i)
                    SetChar(current.UpperLeft + new Point(OffsetToCenterOfSize, i + SquareSize - 1), (char)TCODSpecialCharacter.VertLine);
                return;
            }
            else if (rowDifference == 0 && columnDifference == 1)
            {
                // 2
                for (int i = 1; i <= GapBetweenColumns; ++i)
                    SetChar(current.UpperLeft + new Point(-i, OffsetToCenterOfSize), (char)TCODSpecialCharacter.HorzLine);
                return;
            }
            else if (rowDifference == 0 && columnDifference == -1)
            {
                // 3
                for (int i = 1; i <= GapBetweenColumns; ++i)
                    SetChar(current.UpperLeft + new Point(SquareSize + i, OffsetToCenterOfSize), (char)TCODSpecialCharacter.HorzLine);
                return;
            }
            else if (rowDifference == 1 && columnDifference == 1)
            {
                // 4
                for (int i = 0; i < GapBetweenColumns; ++i)
                    SetChar(dependent.UpperLeft + new Point(SquareSize + i, -i), (char)224);
                return;
            }
            else if (rowDifference == 1 && columnDifference == -1)
            {
                // 5
                for (int i = 0; i < GapBetweenColumns; ++i)
                    SetChar(dependent.UpperLeft + new Point(-i - 1, -i), (char)225);
                return;
            }

            throw new InvalidDataException(string.Format("SkillTreeTabXMLHelper drawing dependency I don't know how to draw - {0} to {1}", current.SkillName, dependent.SkillName));
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Skills")
                throw new System.InvalidOperationException("Bad skill defination file");

            m_tabLoading.DefaultCurorPosition = new Point(int.Parse(reader.GetAttribute("DefaultX")), int.Parse(reader.GetAttribute("DefaultY")));

            m_maxRow = int.Parse(reader.GetAttribute("MaxRow"));

            SkillSquare currentSquare = null;
            bool inIcon = false;
            bool inDependencies = false;
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Skills")
                    break;

                if (reader.LocalName == "Skill")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string name = reader.GetAttribute("Name");
                        int column = int.Parse(reader.GetAttribute("Column"));
                        int row = int.Parse(reader.GetAttribute("Row"));
                        currentSquare = new SkillSquare(CalculatePosition(column, row), name);
                        m_tabLoading.SkillSquares.Add(currentSquare);
                        HandleNewPoint(column, row);
                    }
                }
                else if (reader.LocalName == "Icon")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        inIcon = true;
                    else if (reader.NodeType == XmlNodeType.EndElement)
                        inIcon = false;
                }
                else if (reader.LocalName.StartsWith("Row") && reader.NodeType == XmlNodeType.Element)
                {
                    // reader.ReadElementContentAsString() apparently moves us to the next row, so do this while we have a row
                    do
                    {
                        if (!inIcon)
                            throw new InvalidOperationException("SkillTreeTabXMLHelper - Found a row outside of an icon?");
                        int rowNumber = int.Parse(reader.LocalName.Substring(3));

                        string value = reader.ReadElementContentAsString();
                        if (rowNumber != 0 && rowNumber != 1 && rowNumber != 2 && rowNumber != 3)
                            throw new InvalidOperationException("SkillTreeTabXMLHelper - Invalid row in icon?");

                        // Icon text is offset from the upper left
                        Point position = currentSquare.UpperLeft + new Point(1, 2 + rowNumber);
                        m_iconTextLines.Add(new Pair<Point, string>(position, value));
                    }
                    while (reader.LocalName.StartsWith("Row") && reader.NodeType == XmlNodeType.Element);
                }
                else if (reader.LocalName == "Dependencies" && reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        inDependencies = true;
                    else if (reader.NodeType == XmlNodeType.EndElement)
                        inDependencies = false;
                }
                else if (reader.LocalName == "Dependency" && reader.NodeType == XmlNodeType.Element)
                {
                    if (!inDependencies)
                        throw new InvalidOperationException("SkillTreeTabXMLHelper - Found a dependency outside of a dependencies?");

                    currentSquare.AddDependency(reader.GetAttribute("Name"));
                }
            }
        }
    }
}
