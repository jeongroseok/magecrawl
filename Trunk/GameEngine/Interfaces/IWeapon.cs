using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IWeapon : IItem, ITargetablePoints
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

        bool PositionInTargetablePoints(Point pointOfInterest);
        float EffectiveStrengthAtPoint(Point pointOfInterest);
    }
}
