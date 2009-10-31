using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal class Poison : AffectBase
    {        
        public Poison(int totalCT, int damageInterval, int damagePerInterval)
            :base(totalCT)
        {
            m_damageInterval = damageInterval;
            m_damagePerInterval = damagePerInterval;
        }

        private Character m_affected;
        private int m_damageInterval, m_damagePerInterval;

        public override void Apply(Character appliedTo)
        {
            m_affected = appliedTo;
        }

        public override void DecreaseCT(int decrease)
        {
            int original = CTLeft + decrease;
            for (int i = original; i >= CTLeft; i -= m_damageInterval)
            {
                CoreGameEngine.Instance.CombatEngine.DamageTarget(m_damagePerInterval, null, m_affected, null);
            }
        }

        public override void Remove(Character removedFrom)
        {            
        }
    }
}
