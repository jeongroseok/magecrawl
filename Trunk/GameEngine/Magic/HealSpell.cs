using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal class HealSpell : SpellBase
    {
        internal override string Name
        {
            get 
            {
                return "Heal";
            }
        }

        internal override string EffectType
        {
            get
            {
                return "HealCaster";
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
