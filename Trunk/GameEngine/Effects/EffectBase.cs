using System.Xml.Serialization;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Effects
{
    internal abstract class EffectBase : IXmlSerializable
    {
        public abstract void Apply(Character appliedTo);
        public abstract void Remove(Character removedFrom);
        
        // Should match entry in EffectFactory
        public abstract string Name
        {
            get;
        }

        public EffectBase()
        {
            CTLeft = 0;
        }

        public EffectBase(int totalCT)
        {
            CTLeft = totalCT;
        }

        public int CTLeft { get; protected set; }

        public virtual void DecreaseCT(int decrease)
        {
            CTLeft -= decrease;
        }

        public void Extend(double ratio)
        {
            CTLeft = (int)(CTLeft * ratio);
        }

        public void Dismiss()
        {
            CTLeft = 0;
        }

        virtual public bool ProvidesEquipment(IArmor armor)
        {
            return false;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            CTLeft = reader.ReadElementContentAsInt();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", Name);
            writer.WriteElementString("CTLeft", CTLeft.ToString());
        }

        #endregion
    }
}
