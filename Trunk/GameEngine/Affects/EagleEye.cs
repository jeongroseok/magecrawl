using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal class EagleEye : AffectBase
    {
        private int m_strength;

        public EagleEye() : base(0)
        {
        }

        public EagleEye(int strength)
            : base(new DiceRoll(strength, 10, 1).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_strength = strength;
        }

        public override void Apply(Character appliedTo)
        {
            appliedTo.Vision += (m_strength / 2);
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.Vision -= (m_strength / 2);
        }

        public override string Name
        {
            get
            {
                return "Eagle Eye";
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_strength = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Strength", m_strength.ToString());
        }

        #endregion
    }
}
