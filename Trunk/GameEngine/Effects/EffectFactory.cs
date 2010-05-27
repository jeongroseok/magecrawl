using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Effects
{
    public class EffectFactory
    {
        internal static EffectBase CreateEffectBaseObject(string affectName)
        {
            return CreateEffect(null, affectName, 0);
        }

        internal static EffectBase CreateEffect(Character caster, string affectName, int level)
        {
            // MEF?
            switch (affectName)
            {
                case "Haste":
                    return new Haste(level);
                case "Slow":
                    return new Slow(level);
                case "Light":
                    return new Light(level);
                case "Poison":
                {
                    bool castByPlayer = caster is Player;
                    return new Poison(level, castByPlayer);
                }
                case "Earthen Armor":
                    return new EarthenArmor(level);
                default:
                    return null;
            }
        }
    }
}
