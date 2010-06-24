using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Magecrawl.GameEngine.Effects.EffectResults;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.Effects
{
    [Export(typeof(EffectFactory))]
    internal class EffectFactory
    {
        // Apparently, msvc is ignorant of MEF and throws a warning on MEFed things that are private/protected. Disable warning.
        #pragma warning disable 649
        [ImportMany]
        private Lazy<EffectResult, IDictionary<string, object>>[] m_effectResults;
        #pragma warning restore 649

        public StatusEffect CreateEffectBaseObject(string affectName, bool longTerm)
        {
            return CreateEffect(null, affectName, longTerm, 0);
        }

        public StatusEffect CreateEffect(ICharacter caster, string effectName, bool longTerm, int level)
        {
            EffectResult effectResult = null;
            foreach (var v in m_effectResults)
            {
                if ((string)v.Metadata["Name"] == effectName)
                {
                    if (v.Metadata.ContainsKey("Constructor") && (string)v.Metadata["Constructor"] == "CastByPlayer")
                        effectResult = (EffectResult)Activator.CreateInstance(v.Value.GetType(), level, caster is IPlayer);
                    else
                        effectResult = (EffectResult)Activator.CreateInstance(v.Value.GetType(), level);
                    break;
                }
            }
            if (effectResult == null)
                throw new System.InvalidOperationException("Create Effect: Don't know how to create: " + effectName);
            
            StatusEffect effect;
            if (longTerm)
                effect = new LongTermEffect(effectResult);
            else
                effect = new ShortTermEffect(effectResult);
            effect.SetDefaults();
            return effect;
        }
    }
}
