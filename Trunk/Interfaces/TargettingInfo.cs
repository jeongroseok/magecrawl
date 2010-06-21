using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.Interfaces
{
    public class TargetingInfo
    {
        public enum TargettingType
        {
            Self, 
            RangedSingle, 
            RangedBlast, 
            Cone, 
            RangedExplodingPoint 
        }
        
        public TargettingType Type { get; set; }
        public int Range { get; set; }

        public TargetingInfo(TargettingType targetingType)
            : this(targetingType, -1)
        {
        }

        public TargetingInfo(TargettingType targetingType, int range)
        {
            Type = targetingType;
            Range = range;
        }
    }
}
