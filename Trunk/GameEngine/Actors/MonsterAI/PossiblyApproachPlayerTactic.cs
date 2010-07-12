using libtcod;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal class PossiblyApproachPlayerTactic : TacticWithCooldown
    {
        private static TCODRandom s_random = new TCODRandom();
        private const int ChanceToApproach = 50;

        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
            return true;
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
        {
            if (s_random.Chance(50))
                return MoveTowardsPlayer(engine, monster);
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
