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
                if (DoEffect(caster, spell, spell.EffectType, spell.Strength, target, effectString))
                {
                    caster.CurrentMP -= spell.Cost;
                    return true;
                }
            }
            return false;
        }

        internal void UseItemWithEffect(Character invoker, ItemWithEffects item, Point targetedPoint)
        {
            string effectString = string.Format(item.OnUseString, invoker.Name, item.Name);
            DoEffect(invoker, item, item.EffectType, item.Strength, targetedPoint, effectString);
            return;
        }

        public List<Point> SpellCastDrawablePoints(Spell spell, Point target)
        {
            List<Point> returnList = null;
            switch (spell.EffectType)
            {
                case "RangedSingleTarget":
                {
                    returnList = m_physicsEngine.GenerateRangedAttackListOfPoints(CoreGameEngine.Instance.Map, CoreGameEngine.Instance.Player.Position, target);
                    break;
                }
                case "RangedBlast":
                {
                    returnList = m_physicsEngine.GenerateBlastListOfPoints(CoreGameEngine.Instance.Map, CoreGameEngine.Instance.Player.Position, target, true);
                    TrimPathDueToSpellLength(spell.Strength, returnList);
                    break;
                }
                default:
                    return null;
            }
            if (returnList != null)
            {
                // If we debugging the ranged attack, don't limit to our LOS
                if (!(bool)Preferences.Instance["DebugRangedAttack"]) 
                {
                    TileVisibility[,] visibilityGrid = m_physicsEngine.CalculateTileVisibility();
                    return returnList.Where(p => visibilityGrid[p.X, p.Y] == TileVisibility.Visible).ToList();
                }
            }
            return returnList;
        }

        private bool DoEffect(Character invoker, object invokingMethod, string effect, int strength, Point target, string printOnEffect)
        {
            List<Character> actorList = CoreGameEngine.Instance.Map.Monsters.OfType<Character>().ToList();
            actorList.Add(CoreGameEngine.Instance.Player);
            CoreGameEngine.Instance.SendTextOutput(printOnEffect);
            switch (effect)
            {
                case "Rest":
                {
                    if (m_physicsEngine.DangerPlayerInLOS())
                        return false;
                    CoreGameEngine.Instance.SendTextOutput(string.Format("As the campsite forms, time seems to drift away as {0} begins to relax deeply.", invoker.Name));
                    const int RoundsToRest = 5;
                    for (int i = 0; i < RoundsToRest; ++i)
                    {
                        if (m_physicsEngine.DangerPlayerInLOS())
                        {
                            CoreGameEngine.Instance.SendTextOutput(string.Format("The resting enchantment detects danger and jerks {0} awake as it fades from existence.", invoker.Name));
                            break;
                        }
                        invoker.Heal(invoker.MaxHP / RoundsToRest);
                        ((Player)invoker).HealMP(((Player)invoker).MaxMP / RoundsToRest);
                        
                        m_physicsEngine.Wait(invoker);
                        m_physicsEngine.AfterPlayerAction(CoreGameEngine.Instance);

                        if (i + 1 == RoundsToRest)
                            CoreGameEngine.Instance.SendTextOutput(string.Format("The resting enchantment fades and {0} awake refreshed.", invoker.Name));
                    }
                    return true;
                }
                case "HealCaster":
                {
                    int healAmount = invoker.Heal((new DiceRoll(strength, 6, 2)).Roll());
                    CoreGameEngine.Instance.SendTextOutput(string.Format("{0} was healed for {1} health.", invoker.Name, healAmount));
                    return true;
                }
                case "HealMPCaster":
                {
                    Player player = invoker as Player;
                    if (player != null)
                        player.HealMP((new DiceRoll(strength, 4, 3)).Roll());
                    return true;
                }
                case "RangedSingleTarget":
                {
                    m_combatEngine.RangedBoltToLocation(invoker, target, CalculateDamgeFromSpell(strength), invokingMethod, DamageDoneDelegate);
                    return true;
                }
                case "RangedBlast":
                {
                    List<Point> pathOfBlast = m_physicsEngine.GenerateBlastListOfPoints(CoreGameEngine.Instance.Map, invoker.Position, target, true);
                    TrimPathDueToSpellLength(strength, pathOfBlast);
                    CoreGameEngine.Instance.ShowRangedAttack(invokingMethod, pathOfBlast, m_combatEngine.FindTargetAtPosition(pathOfBlast.Last()) != null);
                    foreach (Point p in pathOfBlast)
                    {
                        Character hitCharacter = m_combatEngine.FindTargetAtPosition(p);
                        if (hitCharacter != null)
                        {
                            m_combatEngine.DamageTarget(CalculateDamgeFromSpell(strength), hitCharacter,
                                new CombatEngine.DamageDoneDelegate(DamageDoneDelegate));
                        }
                    }
                    return true;
                }
                case "Haste":
                case "False Life":
                case "Light":
                case "Earthen Armor":
                {
                    invoker.AddAffect(Affects.AffectFactory.CreateAffect(effect, strength));
                    return true;
                }
                case "Poison Bolt":
                {
                    m_combatEngine.RangedBoltToLocation(invoker, target, 1, invokingMethod, DamageDoneDelegate);
                    Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
                    if (targetCharacter != null)
                        targetCharacter.AddAffect(Affects.AffectFactory.CreateAffect("Poison", strength));
                    return true;
                }
                case "Blink":
                    return HandleRandomTeleport(invoker, 5);
                case "Teleport":
                    return HandleRandomTeleport(invoker, 25);
                case "Slow":
                {
                    Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
                    if (targetCharacter != null)
                        targetCharacter.AddAffect(Affects.AffectFactory.CreateAffect("Slow", strength));
                    return true;
                }
                default:
                    return false;
            }
        }

        private static int CalculateDamgeFromSpell(int strength)
        {
            int damage = 0;
            for (int i = 0; i < strength; ++i)
                damage += (new DiceRoll(1, 4, 0, 1)).Roll();
            return damage;
        }

        private static void TrimPathDueToSpellLength(int strength, List<Point> pathOfBlast)
        {
            int range = Math.Max(2 * strength, 10);
            if (pathOfBlast.Count > range)
                pathOfBlast.RemoveRange(range, pathOfBlast.Count - range);
        }

        private delegate void OnRangedAffect(Character c, int strength);

        private bool HandleRandomTeleport(Character caster, int range)
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
                CoreGameEngine.Instance.SendTextOutput(string.Format("Things become fuzzy as {0} shifts into a new position.", caster.Name));
                m_physicsEngine.WarpToPosition(caster, pointToTeleport.Position);
            }
            return true;
        }

        private static void DamageDoneDelegate(int damage, Character target, bool targetKilled)
        {
            string centerString = targetKilled ? "was killed ({0} damage)" : "took {0} damage";

            string prefix = target.GetType() == typeof(IPlayer) ? string.Empty : "The";
            CoreGameEngine.Instance.SendTextOutput(string.Format("{0} {1} {2}.", prefix, target.Name, string.Format(centerString, damage)));
        }
    }
}
