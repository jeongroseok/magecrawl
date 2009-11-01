using System;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class EagleEye : AffectBase
    {
        private int m_visionBoost;

        public EagleEye() : base(0)
        {
        }

        public EagleEye(int strength)
            : base(new DiceRoll(strength, 10, 1).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
            m_visionBoost = Math.Min(strength / 2, 2);
        }

        public override void Apply(Character appliedTo)
        {
            appliedTo.Vision += m_visionBoost;
        }

        public override void Remove(Character removedFrom)
        {
            removedFrom.Vision -= m_visionBoost;
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
            m_visionBoost = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("VisionBoost", m_visionBoost.ToString());
        }

        #endregion
    }
}
