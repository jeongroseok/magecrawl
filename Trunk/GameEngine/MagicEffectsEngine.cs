using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Magic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class MagicEffectsEngine
    {
        internal MagicEffectsEngine()
        {
        }

        internal bool CastSpell(Character caster, SpellBase spell)
        {
            if (caster.CurrentMP >= spell.Cost)
            {
                DoEffect(caster, spell.EffectType, spell.Strength);
                caster.CurrentMP -= spell.Cost;
                return true;
            }
            return false;
        }

        private void DoEffect(Character caster, string effect, int strength)
        {
            switch (effect)
            {
                case "HealCaster":
                    caster.Heal((new DiceRoll(1, 4, 0, (short)strength)).Roll());
                    break;
                case "AOE Blast Center Caster":
                    CoreGameEngine.Instance.SendTextOutput("Will do blast effect later.");
                    break;
            }
        }
    }
}
