using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    class DefaultTactic : BaseTactic
    {
        public override bool CouldUseTactic(CoreGameEngine engine, Monster monster)
        {
            return true;
        }

        public override bool UseTactic(CoreGameEngine engine, Monster monster)
        {
            if (IsPlayerVisible(engine, monster))
            {
                List<Point> pathToPlayer = GetPathToPlayer(engine, monster);

                if (IsNextToPlayer(engine, pathToPlayer))
                {
                    if (AttackPlayer(engine, monster))
                        return true;
                }

                if (HasPathToPlayer(engine, pathToPlayer))
                {
                    if (MoveOnPath(engine, pathToPlayer, monster))
                        return true;
                }
            }
            else
            {
                if (WalkTowardsLastKnownPosition(engine, monster))
                    return true;
            }
            WanderRandomly(engine, monster);   // We have nothing else to do, so wander                
            return true;
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
