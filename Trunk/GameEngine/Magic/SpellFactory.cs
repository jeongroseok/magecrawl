using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameEngine.Magic
{
    internal class SpellFactory
    {
        public static SpellBase CreateSpell(string spellName)
        {
            switch (spellName)
            {
                case "Heal":
                    return new HealSpell();
                case "Blast":
                    return new BlastSpell();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
