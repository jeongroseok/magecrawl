using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    class WaitTactic : BaseTactic
    {
        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
            return true;
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
        {
            return engine.Wait(monster);
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
