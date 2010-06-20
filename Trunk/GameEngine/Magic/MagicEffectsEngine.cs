using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Effects;
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
                if (DoEffect(caster, spell, spell.EffectType, caster.SpellStrength(spell.School), target, effectString, caster.CurrentMP - spell.Cost))
                {
                    caster.SpendMP(spell.Cost);
                    return true;
                }
            }
            return false;
        }

        internal void UseItemWithEffect(Character invoker, ItemWithEffects item, Point targetedPoint)
        {
            string effectString = string.Format(item.OnUseString, invoker.Name, item.Name);
            DoEffect(invoker, item, item.Spell.EffectType, item.Strength, targetedPoint, effectString, -1);
            return;
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
                    Point playerPosition = CoreGameEngine.Instance.Player.Position;
                    Direction direction = PointDirectionUtils.ConvertTwoPointsToDirection(playerPosition, target);
                    List<Point> returnList = PointListUtils.PointListFromCone(playerPosition, direction, 3);
                    m_physicsEngine.FilterNotTargetablePointsFromList(returnList, playerPosition, CoreGameEngine.Instance.Player.Vision, true);
                    CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(playerPosition, returnList, playerPosition);
                    return returnList;
                }
                case TargetingInfo.TargettingType.RangedExplodingPoint:
                {
                    List<Point> returnList = PointListUtils.PointListFromBurstPosition(target, 2);
                    m_physicsEngine.FilterNotTargetablePointsFromList(returnList, CoreGameEngine.Instance.Player.Position, CoreGameEngine.Instance.Player.Vision, true);
                    CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(CoreGameEngine.Instance.Player.Position, returnList, CoreGameEngine.Instance.Player.Position);
                    return returnList;
                }
                case TargetingInfo.TargettingType.RangedSingle:
                case TargetingInfo.TargettingType.Self:
                default:
                    return null;
            }
        }

        // mpTotalAfterSpellCastIfApplicable is a hack, used to not cast spells that have sustaining costs you can't sustain.
        // DoEffect needs to be refactored, and callers should be able to ask if there will be a sustaining cost pre-cast
        private bool DoEffect(Character invoker, object invokingMethod, string effectName, int strength, Point target, string printOnEffect, int mpTotalAfterSpellCastIfApplicable)
        {
            switch (effectName)
            {                
                case "HealCaster":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    int healAmount = invoker.Heal((new DiceRoll(2 * strength, 6, 1)).Roll(), true);
                    CoreGameEngine.Instance.SendTextOutput(string.Format("{0} was healed for {1} health.", invoker.Name, healAmount));
                    return true;
                }
                case "HealMPCaster":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    Player player = invoker as Player;
                    if (player != null)
                        player.GainMP((new DiceRoll(strength, 4, 3)).Roll());
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
                            m_combatEngine.DamageTarget(invoker, CalculateDamgeFromSpell(strength), hitCharacter, DamageDoneDelegate);
                    }
                    return true;
                }
                case "ConeAttack":
                {
                    Direction direction = PointDirectionUtils.ConvertTwoPointsToDirection(invoker.Position, target);
                    List<Point> pointsInConeAttack = PointListUtils.PointListFromCone(invoker.Position, direction, 3);
                    CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(invoker.Position, pointsInConeAttack, invoker.Position);
                    if (pointsInConeAttack == null || pointsInConeAttack.Count == 0)
                        return false;   // Nothing to roast, not sure how we could get here however...

                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    ShowConeAttack(invoker, invokingMethod, pointsInConeAttack);

                    foreach (Point p in pointsInConeAttack)
                    {
                        Character hitCharacter = m_combatEngine.FindTargetAtPosition(p);
                        if (hitCharacter != null)
                            m_combatEngine.DamageTarget(invoker, CalculateDamgeFromSpell(strength), hitCharacter, DamageDoneDelegate);
                    }
                    return true;
                }
                case "ExplodingRangedPoint":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    const int BurstWidth = 2;
                    
                    ShowExplodingRangedPointAttack(invoker, invokingMethod, target, BurstWidth);

                    List<Point> pointsToEffect = PointListUtils.PointListFromBurstPosition(target, BurstWidth);
                    CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(invoker.Position, pointsToEffect, invoker.Position);
                    foreach (Point p in pointsToEffect)
                    {
                        Character hitCharacter = m_combatEngine.FindTargetAtPosition(p);
                        if (hitCharacter != null)
                            m_combatEngine.DamageTarget(invoker, CalculateDamgeFromSpell(strength), hitCharacter, DamageDoneDelegate);
                    }

                    return true;
                }
                case "Haste":
                case "Light":
                case "Earthen Armor":
                {
                    EffectBase previousEffect = invoker.Effects.FirstOrDefault(x => x.Name == effectName);
                    if (previousEffect == null)
                    {
                        LongTermEffect effect = (LongTermEffect)Effects.EffectFactory.CreateEffect(invoker, effectName, strength);
                        Player invokerAsPlayer = invoker as Player;
                        if (invokerAsPlayer != null)
                        {
                            if(effect.MPCost > mpTotalAfterSpellCastIfApplicable && mpTotalAfterSpellCastIfApplicable != -1)
                            {
                                CoreGameEngine.Instance.SendTextOutput(string.Format("Cannot cast {0} as {1} does not have the energy to sustain it.", effectName, invoker.Name));
                                return false;
                            }
                        }
                        // Check here if mp will bring us under 0 since mp cost of spell hasn't hit yet...
                        CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                        invoker.AddEffect(effect);
                        return true;
                    }
                    else
                    {
                        CoreGameEngine.Instance.SendTextOutput(string.Format("Cannot cast {0} as {1} already has the effect.", effectName, invoker.Name));
                        return false;
                    }
                }
                case "Poison Bolt":
                {
                    CoreGameEngine.Instance.SendTextOutput(printOnEffect);
                    m_combatEngine.RangedBoltToLocation(invoker, target, 1, invokingMethod, DamageDoneDelegate);
                    AddAffactToTarget("Poison", invoker, strength, target);
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
                    AddAffactToTarget("Slow", invoker, strength, target);
                    return true;
                }
                default:
                    throw new InvalidOperationException("MagicEffectsEngine::DoEffect - don't know how to do: " + effectName);
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
                CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(invoker.Position, explosionRing, invoker.Position);
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

        private void AddAffactToTarget(string name, Character caster, int strength, Point target)
        {
            Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
            if (targetCharacter != null)
                targetCharacter.AddEffect(Effects.EffectFactory.CreateEffect(caster, name, strength));
        }

        private static int CalculateDamgeFromSpell(int strength)
        {
            int damage = 0;
            damage += (new DiceRoll(3, 3, 0, 1)).Roll();
            for (int i = 1; i < strength; ++i)
                damage += (new DiceRoll(1, 3, 0, 1)).Roll();
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

        private delegate void OnRangedEffect(Character c, int strength);

        private bool HandleRandomTeleport(Character caster, int range)
        {
            List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(caster.Position, range);
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, caster.Position, caster.Vision, false);

            // If there's no where to go, we're done
            if (targetablePoints.Count == 0)
                return true;
            using (TCODRandom random = new TCODRandom())
            {
                int element = random.getInt(0, targetablePoints.Count - 1);
                EffectivePoint pointToTeleport = targetablePoints[element];
                CoreGameEngine.Instance.SendTextOutput(string.Format("Things become fuzzy as {0} shifts into a new position.", caster.Name));
                m_physicsEngine.WarpToPosition(caster, pointToTeleport.Position);
            }
            return true;
        }

        private static void DamageDoneDelegate(int damage, Character target, bool targetKilled)
        {
            string centerString = targetKilled ? "was killed ({0} damage)" : "took {0} damage";

            string prefix = target.GetType() == typeof(IPlayer) ? "" : "The";
            CoreGameEngine.Instance.SendTextOutput(string.Format("{0} {1} {2}.", prefix, target.Name, string.Format(centerString, damage)));
        }
    }
}
