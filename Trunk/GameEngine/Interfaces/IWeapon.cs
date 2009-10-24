using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IWeapon
    {
        DiceRoll Damage
        {
            get;
        }

        string Name
        {
            get;
        }

        List<Point> TargetablePoints(Point characterPosition);
    }
}
