using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
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
