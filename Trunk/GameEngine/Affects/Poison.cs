using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class Poison : AffectBase
    {
        public Poison() : base(0)
        {
        }

        public Poison(int strength)
            : base(new DiceRoll(1, 4, strength).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_damagePerInterval = strength / 3;
            if (m_damagePerInterval == 0)
                m_damagePerInterval = 1;
        }

        private Character m_affected;
        private int m_damagePerInterval;

        public override void Apply(Character appliedTo)
        {
            m_affected = appliedTo;
        }

        public override void DecreaseCT(int decrease)
        {
            base.DecreaseCT(decrease);
            int original = CTLeft + decrease;
            for (int i = original - 1; i >= CTLeft; i--)
            {
                if (i % CoreTimingEngine.CTNeededForNewTurn == 0)
                {
                    CoreGameEngine.Instance.CombatEngine.DamageTarget(m_damagePerInterval, m_affected, null);
                }
            }            
        }

        public override void Remove(Character removedFrom)
        {            
        }

        public override string Name
        {
            get 
            {
                return "Poison";
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_damagePerInterval = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("DamagePerInterval", m_damagePerInterval.ToString());
        }
    }
}
