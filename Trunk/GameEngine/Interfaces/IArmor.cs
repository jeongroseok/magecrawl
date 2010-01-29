using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameEngine.Interfaces
{
    public enum ArmorWeight
    {
        Light,
        Standard,
        Heavy
    }
    public interface IArmor : INamedItem
    {
        ArmorWeight Weight
        {
            get;
        }

        double Defense
        {
            get;
        }

        double Evade
        {
            get;
        }
    }
}
