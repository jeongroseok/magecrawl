using System;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.StatusEffects.EffectResults;
using Magecrawl.Utilities;

namespace Magecrawl.StatusEffects
{
    public class EffectFactory
    {        
        public static IStatusEffectCore CreateEffectBaseObject(string affectName, bool longTerm)
        {
            return CreateEffect(null, affectName, longTerm, 0);
        }

        public static IStatusEffectCore CreateEffect(ICharacter caster, string effectName, bool longTerm, int strength)
        {
            EffectResult effectResult = null;
            Type effectType = TypeLocator.GetTypeToMake(typeof(EffectFactory), "Magecrawl.StatusEffects.EffectResults", effectName);
            if (effectType == null)
                throw new System.InvalidOperationException("Create Effect: Don't know how to create type of : " + effectName);

            effectResult = (EffectResult)Activator.CreateInstance(effectType, strength, caster);

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
