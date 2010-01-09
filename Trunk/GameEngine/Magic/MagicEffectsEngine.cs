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
        private CombatEngine m_combatEngine;
        private PhysicsEngine m_physicsEngine;

        internal MagicEffectsEngine(PhysicsEngine physicsEngine, CombatEngine combatEngine)
        {
            m_combatEngine = combatEngine;
            m_physicsEngine = physicsEngine;
        }

        internal bool CastSpell(Player caster, Spell spell, Point target)
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
                    Player player = caster as Player;
                    if(player != null)
                    {
                        CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                        player.CurrentMP += (new DiceRoll(strength, 4, 2)).Roll();
                        if (player.CurrentMP > player.MaxMP)
                            player.CurrentMP = player.MaxMP;
                    }
                    return true;
                }
                case "Ranged Single Target":
                {
                    OnRangedAffect rangedAttackDelegate = (c, s) =>
                    {
                        int damage = (new DiceRoll(1, 3, 0, s)).Roll();
                        m_combatEngine.DamageTarget(damage, c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                    };
                    return HandleRangedAffect(caster, strength, target, printOnEffect, actorList, rangedAttackDelegate);
                }
                case "Haste":
                case "False Life":
                case "Eagle Eye":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    caster.AddAffect(Affects.AffectFactory.CreateAffect(effect, strength));
                    return true;
                }
                case "Poison Bolt":
                {
                    OnRangedAffect poisonBoltHitDelegate = (c, s) =>
                    {
                        m_combatEngine.DamageTarget(1, c, new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
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

        private bool HandleTeleport(Character caster, int range)
        {
            List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(caster.Position, range);
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, caster.Position, caster.Vision, false);

            // If there's no where to go, we're done
            if (targetablePoints.Count == 0)
                return true;
            using (TCODRandom random = new TCODRandom())
            {
                int element = random.GetRandomInt(0, targetablePoints.Count - 1);
                EffectivePoint pointToTeleport = targetablePoints[element];
                CoreGameEngine.Instance.SendTextOutput("Things become fuzzy as you shift into a new position.");
                m_physicsEngine.WarpToPosition(caster, pointToTeleport.Position);
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
