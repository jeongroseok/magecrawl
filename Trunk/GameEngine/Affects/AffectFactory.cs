using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameEngine.Affects
{
    public class AffectFactory
    {
        internal static AffectBase CreateAffectBaseObject(string affectName)
        {
            return CreateAffect(affectName, 0);
        }

        internal static AffectBase CreateAffect(string affectName, int level)
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
                    return new Poison(level);
                case "Earthen Armor":
                    return new EarthenArmor(level);
                default:
                    return null;
            }
        }
    }
}
