using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class Haste : AffectBase
    {
        public Haste()
            : base(new DiceRoll(1, 4).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
        }

        public override void Apply(Character appliedTo)
        {
            appliedTo.CTIncreaseModifier *= 1.5;
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.CTIncreaseModifier /= 1.5;
        }
    }
}
