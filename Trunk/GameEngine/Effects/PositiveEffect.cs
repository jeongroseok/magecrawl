using System;

namespace Magecrawl.GameEngine.Effects
{
    internal abstract class PositiveEffect : EffectBase
    {
        public PositiveEffect()
        {
            MPCost = 0;
        }

        public PositiveEffect(int mpCost)
        {
            MPCost = mpCost;
        }

        public override void Dismiss()
        {
            MPCost = -1;
        }

        public int MPCost { get; internal set; }
    }
}
