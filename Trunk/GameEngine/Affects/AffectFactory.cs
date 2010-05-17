using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Affects
{
    public class AffectFactory
    {
        internal static AffectBase CreateAffectBaseObject(string affectName)
        {
            return CreateAffect(null, affectName, 0);
        }

        internal static AffectBase CreateAffect(Character caster, string affectName, int level)
        {
            // MEF?
            switch (affectName)
            {
                case "Haste":
                    return new Haste(level);
                case "Slow":
                    return new Slow(level);
                case "False Life":
                    return new FalseLife(level);
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
