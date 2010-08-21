using Magecrawl.StatusEffects.EffectResults;
using Magecrawl.StatusEffects.Interfaces;

namespace Magecrawl.StatusEffects
{
    internal class LongTermEffect : StatusEffect, ILongTermStatusEffect
    {
        public int MPCost { get; set; }
        public bool Dismissed { get; private set; }

        public LongTermEffect()
        {
            MPCost = 0;
            Dismissed = false;
            m_effectResult = null;
        }

        public LongTermEffect(EffectResult effect)
        {
            MPCost = 0;
            Dismissed = false;
            m_effectResult = effect;
        }

        public override void Dismiss()
        {
            Dismissed = true;
        }

        internal override void SetDefaults()
        {
            if (m_effectResult.DefaultMPSustainingCost == -1)
                throw new System.InvalidOperationException("Trying to sustain an effect with an invalid cost");
            MPCost = m_effectResult.DefaultMPSustainingCost;
        }
    }
}
