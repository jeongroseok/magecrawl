using System;
using System.Xml;

namespace Magecrawl.GameEngine.Effects
{
    internal abstract class NegativeEffect : EffectBase
    {
        public NegativeEffect()
        {
            CTLeft = 0;
        }

        public NegativeEffect(int totalCT)
        {
            CTLeft = totalCT;
        }

        public int CTLeft { get; protected set; }

        public void Extend(double ratio)
        {
            CTLeft = (int)(CTLeft * ratio);
        }

        public virtual void DecreaseCT(int decrease)
        {
            CTLeft -= decrease;
        }

        public override void Dismiss()
        {
            CTLeft = 0;
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
