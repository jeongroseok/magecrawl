using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.Interfaces
{
    public interface IWeaponRange
    {
        List<EffectivePoint> CalculateTargetablePoints(IWeapon weapon, Point wielderPosition);
        bool IsRanged { get; }
        string Name { get; }
    }
}
