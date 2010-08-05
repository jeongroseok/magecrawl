using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Effects.EffectResults;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.Effects
{
    internal abstract class StatusEffect : IStatusEffect, IXmlSerializable
    {
        protected EffectResult m_effectResult;

        public void Apply(Character appliedTo)
        {
            m_effectResult.Apply(appliedTo);
        }

        public void Remove(Character removedFrom)
        {
            m_effectResult.Remove(removedFrom);
        }

        public virtual bool ContainsKey(string key)
        {
            return m_effectResult.ContainsKey(key);
        }

        public virtual string GetAttribute(string key)
        {
            return m_effectResult.GetAttribute(key);
        }
        
        // Should match entry in EffectFactory
        public string Name
        {
            get
            {
                return m_effectResult.Name;
            }
        }

        public string Type
        {
            get
            {
                return m_effectResult.Type;
            }
        }

        public string DisplayName
        {
            get
            {
                return Name;
            }
        }

        public bool IsPositiveEffect
        {
            get
            {
                return m_effectResult.IsPositiveEffect;
            }
        }

        public bool ProvidesEquipment(IArmor armor)
        {
            return m_effectResult.ProvidesEquipment(armor);
        }

        internal abstract void SetDefaults();

        internal abstract void Dismiss();

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            m_effectResult.ReadXml(reader);
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", Type);
            writer.WriteElementString("LongTerm", this is LongTermEffect ? true.ToString() : false.ToString());
            m_effectResult.WriteXml(writer);
        }

        #endregion
    }
}
