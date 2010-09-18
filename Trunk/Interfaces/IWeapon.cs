using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface IWeapon : IItem
    {
        DiceRoll Damage
        {
            get;
        }

        bool IsRanged
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        double CTCostToAttack
        {
            get;
        }
    }
}
