using System.Collections.Generic;
using System.Xml;
using Magecrawl.Interfaces;
using Magecrawl.EngineInterfaces;

namespace Magecrawl.StatusEffects.EffectResults
{
    internal abstract class EffectResult
    {
        internal virtual void Apply(ICharacterCore appliedTo)
        { 
        }

        internal virtual void Remove(ICharacterCore removedFrom)
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

        internal abstract string Type
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
