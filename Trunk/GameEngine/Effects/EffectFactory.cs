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

        public static StatusEffect CreateEffect(ICharacter caster, string effectName, bool longTerm, int level)
        {
            EffectResult effectResult = null;
            Type effectType = TypeLocator.GetTypeToMake(typeof(EffectFactory), "Magecrawl.GameEngine.Effects.EffectResults", effectName);
            if (effectType == null)
                throw new System.InvalidOperationException("Create Effect: Don't know how to create type of : " + effectName);

            object[] customAttributes = effectType.GetCustomAttributes(typeof(EffectResultAttribute), false);
            if (customAttributes.Length == 0)
            {
                effectResult = (EffectResult)Activator.CreateInstance(effectType, level);
            }
            else
            {
                if (((EffectResultAttribute)customAttributes[0]).Tag == "CastByPlayer")
                    effectResult = (EffectResult)Activator.CreateInstance(effectType, level, caster is IPlayer);
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
