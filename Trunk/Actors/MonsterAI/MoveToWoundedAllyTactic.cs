using System.Collections.Generic;
using System.Linq;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Actors.MonsterAI
{
    internal class MoveToWoundedAllyTactic : TacticWithCooldown
    {
        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            // If see an ally who's wounded (I have something to do)
            List<Character> nearbyAllies = OtherNearbyEnemies(engine, monster);
            foreach (Character allyNeedingHealing in nearbyAllies.Where(x => x.CurrentHP < x.MaxHP).OrderBy(x => x.CurrentHP))
            {
                List<Point> pathToAlly = GetPathToCharacter(engine, monster, allyNeedingHealing);
                if (pathToAlly != null && pathToAlly.Count > 1)
                    return true;
            }
            return false;
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            bool success = false;
            
            // If see an ally who's wounded (I have something to do)
            List<Character> nearbyAllies = OtherNearbyEnemies(engine, monster);
            foreach (Character allyNeedingHealing in nearbyAllies.Where(x => x.CurrentHP < x.MaxHP).OrderBy(x => x.CurrentHP))
            {
                List<Point> pathToAlly = GetPathToCharacter(engine, monster, allyNeedingHealing);
                if (pathToAlly != null && pathToAlly.Count > 1)
                {
                    success = MoveOnPath(engine, pathToAlly, monster);
                    if (!success)
                        success = MoveNearbyOnPath(engine, pathToAlly, monster);
                }
            }
            return success;
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
