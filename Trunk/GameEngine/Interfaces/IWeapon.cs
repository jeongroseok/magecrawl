using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    interface IWeapon
    {
        string Name
        {
            get;
        }
        List<Point> TargetablePoints
        {
            get;
        }
    }
}
