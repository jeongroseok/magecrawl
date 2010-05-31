using System;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects
{
    internal class Light : PositiveEffect
    {
        private int m_visionBoost;

        public Light() : base(2)
        {
        }

        public Light(int strength) : base(2)
        {
            m_visionBoost = strength / 2;
            if (m_visionBoost < 2)
                m_visionBoost = 2;
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
                return "Light";
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
