using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class Haste : AffectBase
    {
        private double m_modifier;

        public Haste() : base(0)
        {
        }

        public Haste(int strength)
            : base(new DiceRoll(strength, 6, 2, 2).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_modifier = 1 + (.25 * strength);
        }

        public override void Apply(Character appliedTo)
        {
            appliedTo.CTIncreaseModifier *= m_modifier;
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.CTIncreaseModifier /= m_modifier;
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
            m_modifier = reader.ReadElementContentAsDouble();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Modifier", m_modifier.ToString());
        }

        #endregion
    }
}
