using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Magic
{
    internal abstract class SpellBase
    {
        internal abstract string EffectType
        {
            get;
        }

        internal abstract int Cost
        {
            get;
        }

        internal abstract int Strength
        {
            get;
        }
    }
}
