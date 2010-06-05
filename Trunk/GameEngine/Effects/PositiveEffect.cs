using System;

namespace Magecrawl.GameEngine.Effects
{
    internal abstract class PositiveEffect : EffectBase
    {
        public bool Dismissed { get; private set; }

        public PositiveEffect()
        {
            MPCost = 0;
            Dismissed = false;
        }

        public PositiveEffect(int mpCost)
        {
            MPCost = mpCost;
            Dismissed = false;
        }

        public override bool IsPositiveEffect
        {
            get
            {
                return true;
            }
        }

        public override void Dismiss()
        {
            Dismissed = true;
        }

        public int MPCost { get; internal set; }
    }
}
