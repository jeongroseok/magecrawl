using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class StandardMonster : Monster
    {
        public StandardMonster(string name, Point p, int maxHP, int vision, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, vision, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
        }

        public override MonsterAction Action(CoreGameEngine engine)
        {
            return DefaultAction(engine);
        }
    }
}
