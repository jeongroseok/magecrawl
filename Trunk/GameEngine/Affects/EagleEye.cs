using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal class EagleEye : AffectBase
    {
        public EagleEye() : base(new DiceRoll(2, 10).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
        }

        public override void Apply(Character appliedTo)
        {
            appliedTo.Vision += 4;
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.Vision -= 4;
        }

        public override string Name
        {
            get
            {
                return "Eagle Eye";
            }
        }
    }
}
