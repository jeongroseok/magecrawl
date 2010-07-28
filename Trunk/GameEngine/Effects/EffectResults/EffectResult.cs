using System.Xml;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Interfaces;
using System.Collections.Generic;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal abstract class EffectResult
    {
        internal virtual void Apply(Character appliedTo)
        { 
        }

        internal virtual void Remove(Character removedFrom)
        {
        }

        internal virtual void DecreaseCT(int previousCT, int currentCT)
        {
        }

        public virtual bool ContainsKey(string key)
        {
            return false;
        }

        public virtual string GetAttribute(string key)
        {
            throw new KeyNotFoundException();
        }

        internal abstract string Name 
        {
            get;
        }

        internal abstract bool IsPositiveEffect
        {
            get;
        }

        internal virtual bool ProvidesEquipment(IArmor armor)
        {
            return false;
        }

        internal virtual int DefaultMPSustainingCost
        {
            get
            {
                return -1;
            }
        }

        internal abstract int DefaultEffectLength
        {
            get;
        }

        internal virtual void ReadXml(XmlReader reader)
        {
        }

        internal virtual void WriteXml(XmlWriter writer)
        { 
        }
    }
}
