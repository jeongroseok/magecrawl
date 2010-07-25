using System;
using System.Reflection;
using Magecrawl.GameEngine.Effects.EffectResults;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects
{
    internal class EffectFactory
    {        
        public static StatusEffect CreateEffectBaseObject(string affectName, bool longTerm)
        {
            return CreateEffect(null, affectName, longTerm, 0);
        }

        public static StatusEffect CreateEffect(ICharacter caster, string effectName, bool longTerm, int strength)
        {
            EffectResult effectResult = null;
            Type effectType = TypeLocator.GetTypeToMake(typeof(EffectFactory), "Magecrawl.GameEngine.Effects.EffectResults", effectName);
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
