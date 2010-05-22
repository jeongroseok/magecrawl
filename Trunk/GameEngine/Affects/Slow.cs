using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class Slow : AffectBase
    {
        private double m_modifier;

        public Slow() : base(0)
        {
        }

        public Slow(int strength)
            : base(new DiceRoll(4, 2, strength).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_modifier = 1.4;
        }

        public override void Apply(Character appliedTo)
        {
            appliedTo.CTIncreaseModifier /= m_modifier;
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.CTIncreaseModifier *= m_modifier;
        }

        public override string Name
        {
            get
            {
                return "Haste";
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_modifier = reader.ReadContentAsDouble();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Modifier", m_modifier.ToString());
        }

        #endregion
    }
}
