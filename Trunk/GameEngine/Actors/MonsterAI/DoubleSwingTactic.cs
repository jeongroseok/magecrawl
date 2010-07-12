using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal class DoubleSwingTactic : BaseTactic
    {
        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
            return IsNextToPlayer(engine, monster);
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
        {
            return engine.UseMonsterSkill(monster, SkillType.DoubleSwing, engine.Player.Position);
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
