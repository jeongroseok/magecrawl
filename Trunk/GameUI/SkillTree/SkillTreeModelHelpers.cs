using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal static class SkillTreeModelHelpers
    {
        public static bool HasDependencyOn(IEnumerable<SkillTreeTab> treeTabs, string parentSkill, string possibleDependency)
        {
            foreach (SkillTreeTab tab in treeTabs)
            {
                SkillSquare skillSquare = tab.SkillSquares.Find(x => x.SkillName == parentSkill);
                if (skillSquare != null)
                    return skillSquare.DependentSkills.Contains(possibleDependency);
            }
            throw new System.InvalidOperationException("Unable to HasDependencyOn with " + parentSkill);
        }

        public static bool IsSkillSelected(List<ISkill> selectedSkillList, string skillName)
        {
            return selectedSkillList.ToList().Exists(x => x.Name == skillName);
        }

        public static bool HasUnmetDependencies(List<ISkill> selectedSkillList, SkillSquare skill)
        {
            foreach (string dependencyName in skill.DependentSkills)
            {
                if (!IsSkillSelected(selectedSkillList, dependencyName))
                    return true;
            }
            return false;
        }
    }
}
