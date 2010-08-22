using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public enum MonsterSkillType
    {
        Rush,
        DoubleSwing,
        FirstAid,
        SlingStone
    }

    public interface IMonsterSkillEngine
    {
        bool UseSkill(ICharacterCore invoker, MonsterSkillType skill, Point target);
    }
}
