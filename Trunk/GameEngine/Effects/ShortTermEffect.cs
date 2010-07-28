using System;
using System.Xml;
using Magecrawl.GameEngine.Effects.EffectResults;

namespace Magecrawl.GameEngine.Effects
{
    internal class ShortTermEffect : StatusEffect
    {
        public ShortTermEffect()
        {
            CTLeft = 0;
            m_effectResult = null;
        }

        public ShortTermEffect(EffectResult effect)
        {
            CTLeft = 0;
            m_effectResult = effect;
        }

        public int CTLeft { get; set; }

        public void Extend(double ratio)
        {
            CTLeft = (int)(CTLeft * ratio);
        }

        public virtual void DecreaseCT(int previousCT, int currentCT)
        {
            CTLeft -= previousCT - currentCT;
            m_effectResult.DecreaseCT(previousCT, currentCT);
        }

        internal override void Dismiss()
        {
            CTLeft = 0;
        }

        internal override void SetDefaults()
        {
            CTLeft = m_effectResult.DefaultEffectLength;
        }

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            CTLeft = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("CTLeft", CTLeft.ToString());
        }

        #endregion
    }
}
