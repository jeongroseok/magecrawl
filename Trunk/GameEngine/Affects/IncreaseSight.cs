using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class IncreaseSight : AffectBase
    {
        public IncreaseSight() : base(new DiceRoll(2, 10).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
        }

        public override void Apply(Magecrawl.GameEngine.Actors.Character appliedTo)
        {
            appliedTo.Vision += 4;
        }

        public override void Remove(Magecrawl.GameEngine.Actors.Character removedFrom)
        {
            removedFrom.Vision -= 4;
        }
    }
}
