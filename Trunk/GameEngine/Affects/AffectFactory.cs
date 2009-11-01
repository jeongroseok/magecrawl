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
            switch (affectName)
            {
                case "Haste":
                    return new Haste(level);
                case "False Life":
                    return new FalseLife(level);
                case "Eagle Eye":
                    return new EagleEye(level);
                case "Poison":
                    return new Poison(level);
                default:
                    return null;
            }
        }
    }
}
