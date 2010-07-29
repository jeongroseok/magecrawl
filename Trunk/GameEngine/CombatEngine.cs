using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Level;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class CombatEngine : IDisposable
    {
        public delegate void DamageDoneDelegate(int damage, Character target, bool targetKilled);

        // Cared and fed by CoreGameEngine, local copy for convenience
        private Player m_player;
        private Map m_map;
        private PhysicsEngine m_physicsEngine;

        private TCODRandom m_random;

        internal CombatEngine(PhysicsEngine engine, Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_physicsEngine = engine;
            m_random = new TCODRandom();
        }

        public void Dispose()
        {
            if (m_random != null)
                m_random.Dispose();
            m_random = null;
        }

        internal void NewMapPlayerInfo(Player player, Map map)
        {
            m_player = player;
            m_map = map;        
        }

        internal bool Attack(Character attacker, Point attackTarget)
        {
            if (!IsPositionInTargetablePoints((Weapon)attacker.CurrentWeapon, attacker.Position, attacker.Vision, attackTarget))
                throw new ArgumentException("CombatEngine attacking something current weapon can't attack with?");

            if (attacker.CurrentWeapon.IsRanged)
            {
                if (!attacker.CurrentWeapon.IsLoaded)
                    throw new ArgumentException("CombatEngine attacking something with current weapon unloaded?");

                AttackRanged(attacker, attackTarget, attacker.CurrentWeapon,
                    (dmg, target, killed) => CoreGameEngine.Instance.SendTextOutput(CreateDamageString(dmg, attacker, target)));
                ((Weapon)attacker.CurrentWeapon).UnloadWeapon();
            }
            else
            {
                Character attackedCharacter = FindTargetAtPosition(attackTarget);
                if (attackedCharacter != null)
                {
                    int damageDone = CalculateWeaponStrike(attacker, attackedCharacter);
                    CoreGameEngine.Instance.SendTextOutput(CreateDamageString(damageDone, attacker, attackedCharacter));
                    DamageTarget(attacker, damageDone, attackedCharacter);
                }
            }

            return true;
        }

        private void AttackRanged(Character attacker, Point attackTarget, object attackingMethod, DamageDoneDelegate del)
        {
            Character targetCharacter = FindTargetAtPosition(attackTarget);
            int damageDone = CalculateWeaponStrike(attacker, targetCharacter);
            RangedBoltToLocation(attacker, attackTarget, damageDone, attackingMethod, del);
        }

        private int CalculateWeaponStrike(Character attacker, Character attackedCharacter)
        {
            if (attackedCharacter == null)
                return -1;

            // First check against evade
            int evadeRoll = m_random.getInt(0, 99);

            if ((bool)Preferences.Instance["ShowAttackRolls"])
            {
                CoreGameEngine.Instance.SendTextOutput(string.Format("{0} rolls to hit {1}. Rolled {2} needs above {3} to hit.",
                    attacker.Name, attackedCharacter.Name, evadeRoll, attackedCharacter.Evade));
            }

            if (evadeRoll < attackedCharacter.Evade)
                return -1;

            // Calculate damage
            float effectiveStrength = EffectiveStrengthAtPoint((Weapon)attacker.CurrentWeapon, attacker.Position, attacker.Vision, attackedCharacter.Position);
            double damageDone = (int)Math.Round(attacker.CurrentWeapon.Damage.Roll() * effectiveStrength);
            damageDone += attacker.GetTotalAttributeValue("BonusWeaponDamage");

            damageDone -= attackedCharacter.GetTotalAttributeValue("DamageReduction");

            // If for some reason damage reduction reduces to < 0, do zero damage.
            damageDone = Math.Max(0, damageDone);

            return (int)Math.Round(damageDone);
        }
       
        internal bool RangedBoltToLocation(Character attacker, Point target, int damageDone, object attackingMethod, DamageDoneDelegate del)
        {
            if (attacker.Position == target)
                return false;

            List<Point> attackPath = m_physicsEngine.GenerateRangedAttackListOfPoints(m_map, attacker.Position, target);

            Character targetCharacter = FindTargetAtPosition(target);
            CoreGameEngine.Instance.ShowRangedAttack(attackingMethod, ShowRangedAttackType.RangedBolt, attackPath, targetCharacter != null);

            if (targetCharacter != null)
                DamageTarget(attacker, damageDone, targetCharacter, del);

            if (targetCharacter is Monster)
                ((Monster)targetCharacter).NoticeRangedAttack(attacker.Position);

            return true;
        }

        public Character FindTargetAtPosition(Point attackTarget)
        {
            Monster attackedMonster = (Monster)m_map.Monsters.SingleOrDefault(x => x.Position == attackTarget);
            if (attackedMonster != null)
                return attackedMonster;
            if (attackTarget == m_player.Position)
                return m_player;
            return null;
        }

        public void DamageTarget(Character attacker, int damage, Character target)
        {
            DamageTarget(attacker, damage, target, null);
        }

        public void DamageTarget(Character attacker, int damage, Character target, DamageDoneDelegate del)
        {
            // Sometimes bouncy spells and other things can hit a creature two or more times.
            // If the creature is dead and the map agrees, return early, since the poor sob is already dead and gone.
            if (target.CurrentHP <= 0 && !m_map.Monsters.Contains(target))
                return;

            // -1 damage is coded for a miss
            if (damage == -1)
                return;

            target.Damage(damage);
            bool targetKilled = target.CurrentHP <= 0;
            
            if (del != null)
                del(damage, target, targetKilled);

            if (targetKilled)
            {
                if (target is Monster)
                {
                    if (attacker is Player)
                        ((Player)attacker).SkillPoints += 1;
                    m_map.RemoveMonster(target as Monster);
                    TreasureGenerator.DropTreasureFromMonster(target as Monster);
                }
                else if (target is Player)
                {
                    CoreGameEngine.Instance.PlayerDied();
                }
            }
        }

        public static List<EffectivePoint> CalculateTargetablePointsForEquippedWeapon(Weapon weapon, Point wielderPosition, int wielderVision)
        {
            return CalculateAndFilterTargetablePoints(weapon, wielderPosition, wielderVision);
        }

        private static bool IsPositionInTargetablePoints(Weapon weapon, Point wielderPosition, int wielderVision, Point pointOfInterest)
        {
            foreach (EffectivePoint e in CalculateAndFilterTargetablePoints(weapon, wielderPosition, wielderVision))
            {
                if (e.Position == pointOfInterest)
                    return true;
            }
            return false;
        }

        private static float EffectiveStrengthAtPoint(Weapon weapon, Point wielderPosition, int wielderVision, Point pointOfInterest)
        {
            foreach (EffectivePoint e in CalculateAndFilterTargetablePoints(weapon, wielderPosition, wielderVision))
            {
                if (e.Position == pointOfInterest)
                    return e.EffectiveStrength;
            }
            return 0;
        }

        private static List<EffectivePoint> CalculateAndFilterTargetablePoints(Weapon weapon, Point wielderPosition, int wielderVision)
        {
            List<EffectivePoint> pointList = weapon.CalculateTargetablePoints(wielderPosition);
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(pointList, wielderPosition, wielderVision, true);
            CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(wielderPosition, pointList, Point.Invalid);
            return pointList;
        }

        private string CreateDamageString(int damage, Character attacker, Character defender)
        {
            // "Cheat" to see if attacker or defense is the player to make text output 
            // what is expected. The's should prepend monsters, not player. 
            // If we have 'Proper' named monsters, like say Kyle the Dragon, this will have to be updated.
            bool attackerIsPlayer = attacker is Player;
            bool defenderIsPlayer = defender is Player;
            bool attackKillsTarget = defender.CurrentHP <= damage;

            string verb = ((IWeaponVerb)attacker.CurrentWeapon).AttackVerb;
            if (damage == -1)
            {
                if (attackerIsPlayer)
                    return string.Format("{0} misses the {1}.", attacker.Name, defender.Name);
                else if (defenderIsPlayer)
                    return string.Format("The {0} misses {1}.", attacker.Name, m_player.Name);
                else
                    return string.Format("The {0} misses the {1}.", attacker.Name, defender.Name);
            }
            else if (damage == 0)
            {
                if (attackerIsPlayer)
                    return string.Format("{0} {1} and does no damage to the {2}.", attacker.Name, verb, defender.Name);
                else if (defenderIsPlayer)
                    return string.Format("The {0} {1} and does no damage to {2}.", attacker.Name, verb, defender.Name);
                else
                    return string.Format("The {0} {1} and does no damage to the {2}.", attacker.Name, verb, defender.Name);
            }
            else if (attackKillsTarget)
            {
                if (attackerIsPlayer)
                    return string.Format("{0} {1} and kills the {2} with {3} damage.", attacker.Name, verb, defender.Name, damage.ToString());
                else if (defenderIsPlayer)
                    return string.Format("The {0} {1} and kills {2} with {3} damage.", attacker.Name, verb, defender.Name, damage.ToString());
                else
                    return string.Format("The {0} {1} and kills the {3} with {3} damage.", attacker.Name, verb, defender.Name, damage.ToString());
            }
            else
            {
                if (attackerIsPlayer)
                    return string.Format("{0} {1} the {2} for {3} damage.", attacker.Name, verb, defender.Name, damage.ToString());
                else if (defenderIsPlayer)
                    return string.Format("The {0} {1} {2} for {3} damage.", attacker.Name, verb, defender.Name, damage.ToString());
                else
                    return string.Format("The {0} {1} the {2} for {3} damage.", attacker.Name, verb, defender.Name, damage.ToString());
            }
        }
    }
}
