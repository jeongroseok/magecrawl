using Magecrawl.EngineInterfaces;

namespace Magecrawl.Actors.MonsterAI
{
    internal class KeepAwayFromMeleeRangeIfAbleTactic : TacticWithCooldown
    {
        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            return IsNextToPlayer(engine, monster);
        }

        public override bool UseTactic(IGameEngineCore engine, Monster monster)
        {
            if (AreOtherNearbyEnemies(engine, monster))
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
