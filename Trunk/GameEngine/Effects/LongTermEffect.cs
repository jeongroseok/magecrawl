using Magecrawl.GameEngine.Actors;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.Effects.EffectResults;

namespace Magecrawl.GameEngine.Effects
{
    internal class LongTermEffect : StatusEffect
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

        internal override void Dismiss()
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
