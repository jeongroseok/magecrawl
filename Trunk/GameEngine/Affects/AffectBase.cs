using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    internal abstract class AffectBase : IXmlSerializable
    {
        public abstract void Apply(Character appliedTo);
        public abstract void Remove(Character removedFrom);

        public AffectBase()
        {
            CTLeft = 0;
        }

        public AffectBase(int totalCT)
        {
            CTLeft = totalCT;
        }

        public int CTLeft { get; protected set; }

        public virtual void DecreaseCT(int decrease)
        {
            CTLeft -= decrease;
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
            writer.WriteElementString("Type", this.GetType().Name);
            writer.WriteElementString("CTLeft", CTLeft.ToString());
        }

        #endregion
    }
}
