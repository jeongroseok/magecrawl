using Magecrawl.EngineInterfaces;

namespace Magecrawl.Actors.MonsterAI
{
    internal class UseSlingStoneTactic : TacticWithCooldown
    {
        private const string CooldownName = "SlingCooldown";
        private const int CooldownAmount = 3;

        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            if (CanUseCooldown(monster, CooldownName))
            {
                int range = GetPathToPlayer(engine, monster).Count;
                if (range > 2)
                    return true;
            }
            return false;
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            bool usedSkill = engine.MonsterSkillEngine.UseSkill(monster, MonsterSkillType.SlingStone, engine.Player.Position);
            if (usedSkill)
                UsedCooldown(monster, CooldownName, CooldownAmount);
            return usedSkill;
        }

        public override void SetupAttributesNeeded(Monster monster)
        {
            SetupAttribute(monster, CooldownName, "0");
        }

        public override void NewTurn(Monster monster) 
        {
            NewTurn(monster, CooldownName);
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
