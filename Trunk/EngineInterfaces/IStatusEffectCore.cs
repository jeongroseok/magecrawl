using System.Xml.Serialization;
using Magecrawl.Interfaces;

namespace Magecrawl.EngineInterfaces
{
    public interface IStatusEffectCore : IStatusEffect, IXmlSerializable
    {
        string Type
        {
            get;
        }

        
        void Apply(ICharacterCore appliedTo);
        void Remove(ICharacterCore removedFrom);

        bool ContainsKey(string key);
        string GetAttribute(string key);

        void Dismiss();
    }
}
