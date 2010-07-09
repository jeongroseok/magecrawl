using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    class RushTactic : BaseTactic
    {
        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
           return GetPathToPlayerLength(engine, monster) == 2;
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
        {
            return engine.UseMonsterSkill(monster, SkillType.Rush, engine.Player.Position);
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
