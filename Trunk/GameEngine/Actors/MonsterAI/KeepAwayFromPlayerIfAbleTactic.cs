using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal class KeepAwayFromPlayerIfAbleTactic : TacticWithCooldown
    {        
        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
            return true;
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
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
