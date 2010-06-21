using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface ITargetablePoints
    {
        List<EffectivePoint> CalculateTargetablePoints();
    }
}
