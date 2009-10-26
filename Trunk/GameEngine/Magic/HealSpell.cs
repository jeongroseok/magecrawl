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
        internal override void Cast(Character caster, CombatEngine combatEngine)
        {
            caster.Heal(new DiceRoll(1, 3, 1).Roll());
        }

        internal override int MagicCost
        {
            get
            {
                return 3;
            }
        }

        internal override int Damage
        {
            get 
            {
                return 0;
            }
        }
    }
}
