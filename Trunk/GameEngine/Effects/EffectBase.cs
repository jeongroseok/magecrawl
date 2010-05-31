using System.Xml;
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

        virtual public bool ProvidesEquipment(IArmor armor)
        {
            return false;
        }

        abstract public void Dismiss();

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", Name);
        }

        #endregion
    }
}
