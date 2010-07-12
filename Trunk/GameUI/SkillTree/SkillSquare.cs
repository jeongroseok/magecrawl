using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillSquare
    {
        public Point UpperLeft;
        public Point LowerRight;
        public string SkillName;

        public SkillSquare(Point upperLeft, Point lowerRight)
        {
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
            SkillName = null;
            m_skill = null;
            m_dependentSkills = new List<string>();
        }

        public bool IsInSquare(Point p)
        {
            return p.X > UpperLeft.X && p.X < LowerRight.X && p.Y > UpperLeft.Y && p.Y < LowerRight.Y;
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
    }
}
