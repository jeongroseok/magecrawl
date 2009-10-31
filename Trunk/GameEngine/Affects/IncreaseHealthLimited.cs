using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class IncreaseHealthLimited : AffectBase
    {
        public IncreaseHealthLimited()
            : base(new DiceRoll(2, 4).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
        }

        public override void Apply(Magecrawl.GameEngine.Actors.Character appliedTo)
        {
            appliedTo.MaxHP += 1;
        }

        public override void Remove(Magecrawl.GameEngine.Actors.Character removedFrom)
        {
            removedFrom.MaxHP -= 1;
            removedFrom.CurrentHP = Math.Min(removedFrom.MaxHP, removedFrom.CurrentHP);
        }
    }
}
