using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Effects.EffectResults;

namespace Magecrawl.GameEngine.Effects
{
    internal class LongTermEffect : StatusEffect
    {
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

        public int MPCost { get; set; }        
    }
}
