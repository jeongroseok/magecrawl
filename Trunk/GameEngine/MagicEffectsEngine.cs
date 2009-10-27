using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Magic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class MagicEffectsEngine
    {
        private CombatEngine m_engine;

        internal MagicEffectsEngine(CombatEngine engine)
        {
            m_engine = engine;
        }

        internal bool CastSpell(Character caster, SpellBase spell)
        {
            if (caster.CurrentMP >= spell.Cost)
            {
                CoreGameEngine.Instance.SendTextOutput(string.Format("{0} casts {1}.", caster.Name, spell.Name));
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
                    int healAmount = caster.Heal((new DiceRoll(1, 4, 0, (short)strength)).Roll());
                    CoreGameEngine.Instance.SendTextOutput(string.Format("{0} was healed for {1} health.", caster.Name, healAmount));
                    break;
                case "AOE Blast Center Caster":
                    if (caster is Monster)
                        throw new NotImplementedException("Can't do AOE Blast Center Caster on Monsters yet.");
                    IEnumerable<ICharacter> toDamage = CoreGameEngine.Instance.Map.Monsters.Where(monster => PointDirectionUtils.LatticeDistance(monster.Position, CoreGameEngine.Instance.Player.Position) <= 2);

                    foreach (ICharacter m in toDamage)
                    {
                        int damage = (new DiceRoll(1, 4, 0, (short)strength)).Roll();
                        m_engine.DamageTarget(damage, caster, (Character)m, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                    }
                    break;
            }
        }

        private static void DamageDoneDelegate(int damage, Character target, bool targetKilled)
        {
            string centerString = targetKilled ? "was struck and killed with" : "took";
            CoreGameEngine.Instance.SendTextOutput(string.Format("The {0} {1} {2} damage.", target.Name, centerString, damage));
        }
    }
}
