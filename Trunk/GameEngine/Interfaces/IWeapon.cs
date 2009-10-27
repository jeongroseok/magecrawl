using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public struct EffectivePoint
    {
        public Point Position;

        // On scale of .01 to 1, lower is less damage.
        public float EffectiveStrength;

        public EffectivePoint(Point p, float str)
        {
            Position = p;
            EffectiveStrength = str;
        }
    }

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

        List<EffectivePoint> CalculateTargetablePoints();

        // This version is faster since we don't have to calculate targetablePoints over and over again.
        bool PositionInTargetablePoints(Point pointOfInterest, List<EffectivePoint> targetablePoints);
        bool PositionInTargetablePoints(Point pointOfInterest);

        float EffectiveStrengthAtPoint(Point pointOfInterest);
    }
}
