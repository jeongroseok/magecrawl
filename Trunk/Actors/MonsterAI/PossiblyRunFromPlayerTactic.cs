using System;
using Magecrawl.EngineInterfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Actors.MonsterAI
{
    internal class PossiblyRunFromPlayerTactic : TacticWithCooldown
    {
        private static Random s_random = new Random();
        private const int ChanceToApproach = 50;

        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            return true;
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            if (s_random.Chance(70))
                return MoveAwayFromPlayer(engine, monster);
            return false;
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
