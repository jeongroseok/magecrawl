using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillSquare
    {
        public Point UpperLeft;
        public Point LowerRight;
        public string SkillName { get; private set; }

        public SkillSquare(Point upperLeft, string name)
        {
            UpperLeft = upperLeft;
            LowerRight = upperLeft + new Point(6,6);    // Skill tree is 7x7
            SkillName = name;
            m_skill = null;
            m_dependentSkills = new List<string>();
        }

        public bool IsInSquare(Point p)
        {
            return p.X > UpperLeft.X && p.X < LowerRight.X && p.Y > UpperLeft.Y && p.Y < LowerRight.Y;
        }

        public List<Point> PointsSquareCovers
        {
            get
            {
                List<Point> points = new List<Point>((LowerRight.X - UpperLeft.X) * (LowerRight.Y - UpperRight.Y));
                for (int x = UpperLeft.X + 1; x < LowerRight.X; ++x)
                {
                    for (int y = UpperLeft.Y + 1; y < LowerRight.Y; ++y)
                    {
                        points.Add(new Point(x, y));
                    }
                }
                return points;
            }
        }

        // We do this to cache the skill, since on init time we don't have the IGameEngine to resolve
        private ISkill m_skill;
        public ISkill GetSkill(IGameEngine engine)
        {
            if (m_skill == null)
                m_skill = engine.GameState.GetSkillFromName(SkillName);
            return m_skill;
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

        public override string ToString()
        {
            return "Skill Square - " + SkillName;
        }
    }
}
