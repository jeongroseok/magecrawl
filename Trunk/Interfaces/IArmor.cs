using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.Interfaces
{
    public enum ArmorWeight
    {
        None = 0,
        Light = 1,
        Standard = 2,
        Heavy = 3
    }
    public interface IArmor : IItem, INamedItem
    {
        ArmorWeight Weight
        {
            get;
        }

        int StaminaBonus
        {
            get;
        }

        double Evade
        {
            get;
        }

        bool EquipableByPlayer
        {
            get;
        }
    }
}
