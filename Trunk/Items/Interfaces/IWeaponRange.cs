using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Items.Interfaces
{
    public interface IWeaponRange
    {
        List<EffectivePoint> CalculateTargetablePoints(Point wielderPosition);
        bool IsRanged { get; }
        string Name { get; }
    }
}
