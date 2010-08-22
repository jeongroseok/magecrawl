using Magecrawl.EngineInterfaces;

namespace Magecrawl.Actors.MonsterAI
{
    internal class DoubleSwingTactic : BaseTactic
    {
        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            return IsNextToPlayer(engine, monster);
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            return engine.MonsterSkillEngine.UseSkill(monster, MonsterSkillType.DoubleSwing, engine.Player.Position);
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
