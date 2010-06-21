using System.Xml;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Interfaces;

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

        internal virtual void DecreaseCT(int decrease, int CTLeft)
        {
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
