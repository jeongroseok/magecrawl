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
        internal abstract void Cast(Character caster, CombatEngine combatEngine);

        internal abstract int Damage
        {
            get;
        }

        internal abstract int MagicCost
        {
            get;
        }
    }
}
