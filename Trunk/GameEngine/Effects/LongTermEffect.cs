using System;

namespace Magecrawl.GameEngine.Effects
{
    internal abstract class LongTermEffect : EffectBase
    {
        public bool Dismissed { get; private set; }

        public LongTermEffect()
        {
            MPCost = 0;
            Dismissed = false;
        }

        public LongTermEffect(int mpCost)
        {
            MPCost = mpCost;
            Dismissed = false;
        }

        public override void Dismiss()
        {
            Dismissed = true;
        }

        public int MPCost { get; internal set; }
    }
}
