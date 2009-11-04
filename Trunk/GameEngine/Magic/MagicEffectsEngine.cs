using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
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
                    int healAmount = caster.Heal((new DiceRoll(strength, 4, 1)).Roll());
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    CoreGameEngine.Instance.SendTextOutput(string.Format("{0} was healed for {1} health.", caster.Name, healAmount));
                    return true;
                }
                case "HealMPCaster":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    caster.CurrentMP += (new DiceRoll(strength, 4, 2)).Roll();
                    if (caster.CurrentMP > caster.MaxMP)
                        caster.CurrentMP = caster.MaxMP;
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
                            int damage = (new DiceRoll(2, 3, 0, strength)).Roll();
                            m_engine.DamageTarget(damage, (Character)c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                        }
                    }
                    return true;
                }
                case "Ranged Single Target":
                {
                    OnRangedAffect rangedAttackDelegate = (c, s) =>
                    {
                        int damage = (new DiceRoll(1, 3, 0, s)).Roll();
                        m_engine.DamageTarget(damage, c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                    };
                    return HandleRangedAffect(caster, strength, target, printOnEffect, actorList, rangedAttackDelegate);
                }
                case "Haste":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    caster.AddAffect(Affects.AffectFactory.CreateAffect("Haste", strength));
                    return true;
                }
                case "False Life":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    caster.AddAffect(Affects.AffectFactory.CreateAffect("False Life", strength));
                    return true;
                }
                case "Eagle Eye":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    caster.AddAffect(Affects.AffectFactory.CreateAffect("Eagle Eye", strength));
                    return true;
                }
                case "Poison Bolt":
                {
                    OnRangedAffect poisonBoltHitDelegate = (c, s) =>
                    {
                        m_engine.DamageTarget(1, c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                        c.AddAffect(Affects.AffectFactory.CreateAffect("Poison", strength));
                    };
                    return HandleRangedAffect(caster, strength, target, printOnEffect, actorList, poisonBoltHitDelegate);
                }
                case "Blink":
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    return HandleTeleport(caster, 5);
                case "Teleport":
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    return HandleTeleport(caster, 25);
                case "Slow":
                {
                    OnRangedAffect slowHitDelegate = (c, s) =>
                    {
                        c.AddAffect(Affects.AffectFactory.CreateAffect("Slow", strength));
                    };
                    return HandleRangedAffect(caster, strength, target, printOnEffect, actorList, slowHitDelegate);
                }
                default:
                    return false;
            }
        }

        private delegate void OnRangedAffect(Character c, int strength);

        private bool HandleRangedAffect(Character caster, int strength, Point target, string printOnEffect, List<ICharacter> actorList, OnRangedAffect onHitAction)
        {
            foreach (Character c in actorList)
            {
                if (c.Position == target)
                {
                    if (c != caster)
                    {
                        CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                        onHitAction(c, strength);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool HandleTeleport(Character caster, int range)
        {
            List<EffectivePoint> targetablePoints = PointListUtils.PointListFromBurstPosition(caster.Position, range);
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, caster.Position, caster.Vision, false);

            // If there's no where to go, we're done
            if (targetablePoints.Count == 0)
                return true;
            using (TCODRandom random = new TCODRandom())
            {
                int element = random.GetRandomInt(0, targetablePoints.Count - 1);
                EffectivePoint pointToTeleport = targetablePoints[element];
                CoreGameEngine.Instance.SendTextOutput("Things become fuzzy as you shift into a new position.");
                caster.Position = pointToTeleport.Position;
            }
            return true;
        }

        private static void DamageDoneDelegate(int damage, Character target, bool targetKilled)
        {
            string centerString = targetKilled ? "was struck and killed with" : "took";
            CoreGameEngine.Instance.SendTextOutput(string.Format("The {0} {1} {2} damage.", target.Name, centerString, damage));
        }
    }
}
