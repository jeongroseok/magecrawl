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

        public Poison(int strength, bool castByPlayer)
            : base(new DiceRoll(1, 4, strength).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_damagePerInterval = strength / 3;
            if (m_damagePerInterval == 0)
                m_damagePerInterval = 1;
            m_castByPlayer = castByPlayer;
        }

        private Character m_affected;
        private int m_damagePerInterval;
        private bool m_castByPlayer;

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
                    Character casterWasPlayer = m_castByPlayer ? CoreGameEngine.Instance.Player : null;
                    CoreGameEngine.Instance.CombatEngine.DamageTarget(casterWasPlayer, m_damagePerInterval, m_affected);
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
            m_castByPlayer = reader.ReadElementContentAsBoolean();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("DamagePerInterval", m_damagePerInterval.ToString());
            writer.WriteElementString("CastByPlayer", m_castByPlayer.ToString());
        }
    }
}
