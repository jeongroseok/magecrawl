using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameEngine.Interfaces
{
    public class TargetingInfo
    {
        public enum TargettingType { Self, RangedSingle, RangedBlast, Cone };
        
        public TargettingType Type { get; set; }
        public int Range { get; set; }

        internal TargetingInfo(TargettingType targetingType)
            : this(targetingType, -1)
        {}

        internal TargetingInfo(TargettingType targetingType, int range)
        {
            Type = targetingType;
            Range = range;
        }
    }
}
