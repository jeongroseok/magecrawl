using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal abstract class AffectBase
    {
        public abstract void Apply(Character appliedTo);
        public abstract void Remove(Character removedFrom);

        public AffectBase(int totalCT)
        {
            CTLeft = totalCT;
        }

        public int CTLeft { get; set; }
    }
}
