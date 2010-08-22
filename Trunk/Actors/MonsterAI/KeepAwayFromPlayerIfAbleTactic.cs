using Magecrawl.EngineInterfaces;

namespace Magecrawl.Actors.MonsterAI
{
    internal class KeepAwayFromPlayerWithOtherMonstersNearbyTactic : TacticWithCooldown
    {
        public override bool CouldUseTactic(IGameEngineCore engine, Monster monster)
        {
            return true;
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
