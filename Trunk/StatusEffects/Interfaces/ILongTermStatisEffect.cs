using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.EngineInterfaces;

namespace Magecrawl.StatusEffects.Interfaces
{
    public interface ILongTermStatusEffect : IStatusEffectCore
    {
        int MPCost
        {
            get; 
        }

        bool Dismissed 
        {
            get;
        }
    }
}
