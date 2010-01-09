using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class SprinterMonster : Monster
    {
        public SprinterMonster(string name, Point p, int maxHP, int vision, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, maxHP, vision, ctIncreaseModifer, ctMoveCost, ctActCost)
        {
        }

        public override MonsterAction Action(CoreGameEngine engine)
        {
            return DefaultAction(engine);
        }
    }
}
