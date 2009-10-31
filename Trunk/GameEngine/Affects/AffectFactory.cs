using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameEngine.Affects
{
    public class AffectFactory
    {
        internal static AffectBase CreateAffect(string affectName)
        {
            switch (affectName)
            {
                case "Haste":
                    return new Haste();
                case "IncreaseHealthLimited":
                    return new IncreaseHealthLimited();
                case "IncreaseSight":
                    return new IncreaseSight();
                case "Poison":
                    return new Poison();
                default:
                    return null;
            }
        }
    }
}
