using System.Collections.Generic;
using System.Linq;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Actors.MonsterAI
{
    internal class UseFirstAidTactic : TacticWithCooldown
    {
        private const string CooldownName = "FirstAidCooldown";
        private const int CooldownAmount = 4;

        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            List<Character> nearbyAllies = OtherNearbyEnemies(engine, monster);
            if (nearbyAllies.Count > 0 && CanUseCooldown(monster, CooldownName))
            {
                foreach (Character allyNeedingHealing in nearbyAllies.Where(x => x.CurrentHP < x.MaxHP).OrderBy(x => x.CurrentHP))
                {
                    List<Point> pathToAlly = GetPathToCharacter(engine, monster, allyNeedingHealing);
                    if (pathToAlly != null && pathToAlly.Count <= 1)
                        return true;
                }
            }
            return false;
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            List<Character> nearbyAllies = OtherNearbyEnemies(engine, monster);
            if (nearbyAllies.Count > 0 && CanUseCooldown(monster, CooldownName))
            {
                foreach (Character allyNeedingHealing in nearbyAllies.Where(x => x.CurrentHP < x.MaxHP).OrderBy(x => x.CurrentHP))
                {
                    List<Point> pathToAlly = GetPathToCharacter(engine, monster, allyNeedingHealing);
                    if (pathToAlly != null && pathToAlly.Count <= 1)
                    {
                        engine.MonsterSkillEngine.UseSkill(monster, MonsterSkillType.FirstAid, allyNeedingHealing.Position);
                        UsedCooldown(monster, CooldownName, CooldownAmount);
                        return true;
                    }
                }
            }
            return false;
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
                return false;
            }
        }
    }
}
