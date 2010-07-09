using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal class UseSlingStoneTactic : TacticWithCooldown
    {
        private static string CooldownName = "SlingCooldown";
        private static int CooldownAmount = 3;

        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
            if(CanUseCooldown(monster, CooldownName))
            {
                int range = GetPathToPlayer(engine, monster).Count;
                if (range > 2)
                    return true;
            }
            return false;
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
        {
            return engine.UseMonsterSkill(monster, SkillType.SlingStone, engine.Player.Position);
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
