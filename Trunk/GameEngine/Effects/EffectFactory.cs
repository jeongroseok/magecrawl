using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Effects.EffectResults;

namespace Magecrawl.GameEngine.Effects
{
    public class EffectFactory
    {
        internal static StatusEffect CreateEffectBaseObject(string affectName, bool longTerm)
        {
            return CreateEffect(null, affectName, longTerm, 0);
        }

        internal static StatusEffect CreateEffect(Character caster, string effectName, bool longTerm, int level)
        {
            // MEF
            EffectResult effectResult;
            switch (effectName)
            {
                case "Haste":
                    effectResult = new Haste(level);
                    break;
                case "Slow":
                    effectResult = new Slow(level);
                    break;
                case "Light":
                    effectResult = new Light(level);
                    break;
                case "Poison":
                {
                    bool castByPlayer = caster is Player;
                    effectResult = new Poison(level, castByPlayer);
                    break;
                }
                case "Earthen Armor":
                    effectResult = new EarthenArmor(level);
                    break;
                default:
                    throw new System.InvalidOperationException("CreateEffect can't find - " + effectName);
            }
            
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
