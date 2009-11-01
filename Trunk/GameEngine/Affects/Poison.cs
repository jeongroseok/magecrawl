using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal class Poison : AffectBase
    {
        public Poison()
        {
            m_damageInterval = 1;
            m_damagePerInterval = 0;
        }

        public Poison(int totalCT, int damageInterval, int damagePerInterval) : base(totalCT)
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
            base.DecreaseCT(decrease);
            int original = CTLeft + decrease;
            for (int i = original; i >= CTLeft; i--)
            {
                if (i % m_damageInterval == 0)
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
            m_damageInterval = reader.ReadElementContentAsInt();
            m_damagePerInterval = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("DamageInterval", m_damageInterval.ToString());
            writer.WriteElementString("DamagePerInterval", m_damagePerInterval.ToString());
        }
    }
}
