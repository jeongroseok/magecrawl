using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal class BlastSpell : SpellBase
    {
        internal override string EffectType
        {
            get
            {
                return "AOE Blast Center Caster";
            }
        }

        internal override int Cost
        {
            get
            {
                return 2;
            }
        }

        internal override int Strength
        {
            get
            {
                return 1;
            }
        }
    }
}
