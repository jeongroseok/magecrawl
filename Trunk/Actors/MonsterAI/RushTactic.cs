using Magecrawl.EngineInterfaces;

namespace Magecrawl.Actors.MonsterAI
{
    internal class RushTactic : BaseTactic
    {
        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
           return GetPathToPlayerLength(engine, monster) == 2;
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            return engine.MonsterSkillEngine.UseSkill(monster, MonsterSkillType.Rush, engine.Player.Position);
        }

        public override bool NeedsPlayerLOS
        {
            get
            {
                return true;
            }
        }
    }
}
