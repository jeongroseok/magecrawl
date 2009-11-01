using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Magic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal sealed class MagicEffectsEngine
    {
        private CombatEngine m_engine;

        internal MagicEffectsEngine(CombatEngine engine)
        {
            m_engine = engine;
        }

        internal bool CastSpell(Character caster, Spell spell, Point target)
        {
            if (caster.CurrentMP >= spell.Cost)
            {
                string effectString = string.Format("{0} casts {1}.", caster.Name, spell.Name);
                if (DoEffect(caster, spell.EffectType, spell.Strength, target, effectString))
                {
                    caster.CurrentMP -= spell.Cost;
                    return true;
                }
            }
            return false;
        }

        internal void DrinkPotion(Character drinker, Potion potion)
        {
            string effectString = string.Format("{0} drank the {1}.", drinker.Name, potion.Name);
            DoEffect(drinker, potion.EffectType, potion.Strength, drinker.Position, effectString);
            return;
        }

        private bool DoEffect(Character caster, string effect, int strength, Point target, string printOnEffect)
        {
            List<ICharacter> actorList = new List<ICharacter>(CoreGameEngine.Instance.Map.Monsters);
            actorList.Add(CoreGameEngine.Instance.Player);
            switch (effect)
            {
                case "HealCaster":
                {
                    int healAmount = caster.Heal((new DiceRoll(1, 4, 0, (short)strength)).Roll());
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    CoreGameEngine.Instance.SendTextOutput(string.Format("{0} was healed for {1} health.", caster.Name, healAmount));
                    return true;
                }
                case "AOE Blast Center Caster":
                {
                    if (caster is Monster)
                        throw new NotImplementedException("Can't do AOE Blast Center Caster on Monsters yet.");

                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    IEnumerable<ICharacter> toDamage = CoreGameEngine.Instance.Map.Monsters.Where(monster => PointDirectionUtils.LatticeDistance(monster.Position, CoreGameEngine.Instance.Player.Position) <= 1);

                    foreach (ICharacter c in toDamage)
                    {
                        if (c != caster)
                        {
                            int damage = (new DiceRoll(2, 3, 0, (short)strength)).Roll();
                            m_engine.DamageTarget(damage, (Character)c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                        }
                    }
                    return true;
                }
                case "Ranged Single Target":
                {
                    foreach (ICharacter c in actorList)
                    {
                        if (c.Position == target)
                        {
                            if (c != caster)
                            {
                                CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                                int damage = (new DiceRoll(1, 3, 0, (short)strength)).Roll();
                                m_engine.DamageTarget(damage, (Character)c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                                return true;
                            }
                        }
                    }
                    return false;
                }
                case "Haste":
                {
                    caster.AddAffect(Affects.AffectFactory.CreateAffect("Haste"));
                    return true;
                }
                case "False Life":
                {
                    caster.AddAffect(Affects.AffectFactory.CreateAffect("False Life"));
                    return true;
                }
                case "Increase Sight":
                {
                    caster.AddAffect(Affects.AffectFactory.CreateAffect("Increase Sight"));
                    return true;
                }
                default:
                    return false;
            }
        }

        private static void DamageDoneDelegate(int damage, Character target, bool targetKilled)
        {
            string centerString = targetKilled ? "was struck and killed with" : "took";
            CoreGameEngine.Instance.SendTextOutput(string.Format("The {0} {1} {2} damage.", target.Name, centerString, damage));
        }
    }
}
