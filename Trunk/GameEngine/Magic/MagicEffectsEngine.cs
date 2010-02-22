using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Affects;
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

        internal static TargetingInfo GetAffectTargettingType(string affactString, int strength)
        {
            switch (affactString)
            {
                case "Rest":
                case "HealCaster":
                case "HealMPCaster":
                case "Haste":
                case "False Life":
                case "Light":
                case "Earthen Armor":
                case "Blink":
                case "Teleport":
                    return new TargetingInfo(TargetingInfo.TargettingType.Self);
                case "RangedSingleTarget":
                case "Slow":
                case "Poison Bolt":
                    return new TargetingInfo(TargetingInfo.TargettingType.RangedSingle, 5);
                case "RangedBlast":
                    return new TargetingInfo(TargetingInfo.TargettingType.RangedBlast, 5);
                case "ConeAttack":
                    return new TargetingInfo(TargetingInfo.TargettingType.Cone, 3);
                case "ExplodingRangedPoint":
                    return new TargetingInfo(TargetingInfo.TargettingType.RangedExplodingPoint, 7);
                default:
                    throw new InvalidOperationException("Don't know affect targetting type for: " + affactString);
            }
        }

        internal List<Point> TargettedDrawablePoints(TargetingInfo targeting, int strength, Point target)
        {
            switch (targeting.Type)
            {
                case TargetingInfo.TargettingType.RangedBlast:
                {
                    List<Point> returnList = m_physicsEngine.GenerateBlastListOfPointsShowBounceIfSeeWall(CoreGameEngine.Instance.Map, CoreGameEngine.Instance.Player, target);
                    TrimPathDueToSpellLength(strength, returnList);
                    return returnList;
                }
                case TargetingInfo.TargettingType.Cone:
                {
                    Direction direction = PointDirectionUtils.ConvertTwoPointsToDirection(CoreGameEngine.Instance.Player.Position, target);
                    List<Point> returnList = PointListUtils.PointListFromCone(CoreGameEngine.Instance.Player.Position, direction, 3);
                    m_physicsEngine.FilterNotTargetablePointsFromList(returnList, CoreGameEngine.Instance.Player.Position, CoreGameEngine.Instance.Player.Vision, true);
                    return returnList;
                }
                case TargetingInfo.TargettingType.RangedExplodingPoint:
                {
                    List<Point> returnList = PointListUtils.PointListFromBurstPosition(target, 2);
                    m_physicsEngine.FilterNotTargetablePointsFromList(returnList, CoreGameEngine.Instance.Player.Position, CoreGameEngine.Instance.Player.Vision, true);
                    return returnList;
                }
                case TargetingInfo.TargettingType.RangedSingle:
                case TargetingInfo.TargettingType.Self:
                default:
                    return null;
            }
        }

        private bool DoEffect(Character invoker, object invokingMethod, string effect, int strength, Point target, string printOnEffect)
        {
            List<Character> actorList = CoreGameEngine.Instance.Map.Monsters.OfType<Character>().ToList();
            actorList.Add(CoreGameEngine.Instance.Player);
            switch (effect)
            {
                case "Rest":
                {
                    if (m_physicsEngine.DangerPlayerInLOS())
                        return false;

                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    CoreGameEngine.Instance.SendTextOutput(string.Format("As the campsite forms, time seems to drift away as {0} begins to relax deeply.", invoker.Name));
                    const int RoundsToRest = 5;
                    for (int i = 0; i < RoundsToRest; ++i)
                    {
                        if (m_physicsEngine.DangerPlayerInLOS())
                        {
                            CoreGameEngine.Instance.SendTextOutput(string.Format("The resting enchantment detects danger and jerks {0} awake as it fades from existence.", invoker.Name));
                            return true;
                        }
                        invoker.Heal(invoker.MaxHP / RoundsToRest);
                        ((Player)invoker).HealMP(((Player)invoker).MaxMP / RoundsToRest);
                        
                        m_physicsEngine.Wait(invoker);
                        m_physicsEngine.AfterPlayerAction(CoreGameEngine.Instance);
                    }
                    
                    CoreGameEngine.Instance.SendTextOutput(string.Format("The resting enchantment fades and {0} awake refreshed.", invoker.Name));

                    // Sometimes, due to integer division, we'll be one or two off a full tank
                    invoker.Heal(invoker.MaxHP - invoker.CurrentHP);
                    ((Player)invoker).HealMP(((Player)invoker).MaxMP / ((Player)invoker).CurrentMP);
                    
                    return true;
                }
                case "HealCaster":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    int healAmount = invoker.Heal((new DiceRoll(strength, 6, 2)).Roll());
                    CoreGameEngine.Instance.SendTextOutput(string.Format("{0} was healed for {1} health.", invoker.Name, healAmount));
                    return true;
                }
                case "HealMPCaster":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    Player player = invoker as Player;
                    if (player != null)
                        player.HealMP((new DiceRoll(strength, 4, 3)).Roll());
                    return true;
                }
                case "RangedSingleTarget":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);

                    // This will call ShowRangedAttack inside.
                    m_combatEngine.RangedBoltToLocation(invoker, target, CalculateDamgeFromSpell(strength), invokingMethod, DamageDoneDelegate);
                    return true;
                }
                case "RangedBlast":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    List<Point> pathOfBlast = m_physicsEngine.GenerateBlastListOfPoints(CoreGameEngine.Instance.Map, invoker.Position, target, true);
                    TrimPathDueToSpellLength(strength, pathOfBlast);
                    bool targetAtLastPoint = m_combatEngine.FindTargetAtPosition(pathOfBlast.Last()) != null;
                    CoreGameEngine.Instance.ShowRangedAttack(invokingMethod, ShowRangedAttackType.RangedBoltOrBlast, pathOfBlast, targetAtLastPoint);
                    foreach (Point p in pathOfBlast)
                    {
                        Character hitCharacter = m_combatEngine.FindTargetAtPosition(p);
                        if (hitCharacter != null)
                            m_combatEngine.DamageTarget(CalculateDamgeFromSpell(strength), hitCharacter, DamageDoneDelegate);
                    }
                    return true;
                }
                case "ConeAttack":
                {
                    Direction direction = PointDirectionUtils.ConvertTwoPointsToDirection(invoker.Position, target);
                    List<Point> pointsInConeAttack = PointListUtils.PointListFromCone(invoker.Position, direction, 3);
                    if (pointsInConeAttack == null || pointsInConeAttack.Count == 0)
                        return false;   // Nothing to roast, not sure how we could get here however...

                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    ShowConeAttack(invoker, invokingMethod, pointsInConeAttack);

                    foreach (Point p in pointsInConeAttack)
                    {
                        Character hitCharacter = m_combatEngine.FindTargetAtPosition(p);
                        if (hitCharacter != null)
                            m_combatEngine.DamageTarget(CalculateDamgeFromSpell(strength), hitCharacter, DamageDoneDelegate);
                    }
                    return true;
                }
                case "ExplodingRangedPoint":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    const int burstWidth = 2;
                    
                    ShowExplodingRangedPointAttack(invoker, invokingMethod, target, burstWidth);

                    foreach (Point p in PointListUtils.PointListFromBurstPosition(target, burstWidth))
                    {
                        Character hitCharacter = m_combatEngine.FindTargetAtPosition(p);
                        if (hitCharacter != null)
                            m_combatEngine.DamageTarget(CalculateDamgeFromSpell(strength), hitCharacter, DamageDoneDelegate);
                    }

                    return true;
                }
                case "Haste":
                case "False Life":
                case "Light":
                case "Earthen Armor":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    AffectBase previousAffect = invoker.Affects.Where(x => x.Name == effect).FirstOrDefault();
                    if (previousAffect != null)
                    {
                        previousAffect.Extend(1.5);
                        CoreGameEngine.Instance.SendTextOutput("The previous affect is strengthen in length.");
                    }
                    else
                    {
                        invoker.AddAffect(Affects.AffectFactory.CreateAffect(effect, strength));
                    }
                    return true;
                }
                case "Poison Bolt":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    m_combatEngine.RangedBoltToLocation(invoker, target, 1, invokingMethod, DamageDoneDelegate);
                    AddAffactToTarget("Poison", strength, target);
                    return true;
                }
                case "Blink":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    return HandleRandomTeleport(invoker, 5);
                }
                case "Teleport":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    return HandleRandomTeleport(invoker, 25);
                }
                case "Slow":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    AddAffactToTarget("Slow", strength, target);
                    return true;
                }
                default:
                    throw new InvalidOperationException("MagicEffectsEngine::DoEffect - don't know how to do: " + effect);
            }
        }

        private void ShowExplodingRangedPointAttack(Character invoker, object invokingMethod, Point target, int burstWidth)
        {
            List<Point> pointsInBallPath = RangedAttackPathfinder.RangedListOfPoints(CoreGameEngine.Instance.Map, invoker.Position, target, false, false);

            List<List<Point>> pointsInExplosion = new List<List<Point>>();
            for (int i = 1; i <= burstWidth; ++i)
            {
                List<Point> explosionRing = PointListUtils.PointListFromBurstPosition(target, i);
                CoreGameEngine.Instance.FilterNotTargetablePointsFromList(explosionRing, invoker.Position, invoker.Vision, true);
                pointsInExplosion.Add(explosionRing);
            }

            var pathData = new Pair<List<Point>, List<List<Point>>>(pointsInBallPath, pointsInExplosion);
            CoreGameEngine.Instance.ShowRangedAttack(invokingMethod, ShowRangedAttackType.RangedExplodingPoint, pathData, false);
        }

        private static void ShowConeAttack(Character invoker, object invokingMethod, List<Point> pointsInConeAttack)
        {
            List<Point> coneBlastList = new List<Point>(pointsInConeAttack);
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(coneBlastList, invoker.Position, invoker.Vision, true);            
            CoreGameEngine.Instance.ShowRangedAttack(invokingMethod, ShowRangedAttackType.Cone, coneBlastList, false);
        }

        private void AddAffactToTarget(string name, int strength, Point target)
        {
            Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
            if (targetCharacter != null)
                targetCharacter.AddAffect(Affects.AffectFactory.CreateAffect(name, strength));
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
            if (pathOfBlast == null)
                return;

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
