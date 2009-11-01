using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal class FalseLife : AffectBase
    {
        private int m_lifeIncrease;

        public FalseLife()
            : base(new DiceRoll(2, 4).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_lifeIncrease = -1;
        }

        public override void Apply(Character appliedTo)
        {
            if (m_lifeIncrease == -1)
                m_lifeIncrease = (new DiceRoll(2, 4).Roll());
            
            appliedTo.CurrentHP += m_lifeIncrease;
            appliedTo.MaxHP += m_lifeIncrease;
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.MaxHP -= m_lifeIncrease;
            CoreGameEngine.Instance.CombatEngine.DamageTarget(m_lifeIncrease, removedFrom, null);
        }

        public override string Name
        {
            get
            {
                return "False Life";
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_lifeIncrease = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("LifeIncrease", m_lifeIncrease.ToString());
        }

        #endregion
    }
}
