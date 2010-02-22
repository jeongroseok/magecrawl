using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Level;
using Magecrawl.GameEngine.Weapons;
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
        internal int LastTurnPlayerWasRangedAttacked { get; private set; }

        private TCODRandom m_random;

        internal CombatEngine(PhysicsEngine engine, Player player, Map map)
        {
            m_player = player;
            m_map = map;
            m_physicsEngine = engine;
            LastTurnPlayerWasRangedAttacked = int.MinValue;
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
            if (!attacker.CurrentWeapon.PositionInTargetablePoints(attackTarget))
                throw new ArgumentException("CombatEngine attacking something current weapon can't attack with?");

            if (attacker.CurrentWeapon.IsRanged)
            {
                if (!attacker.CurrentWeapon.IsLoaded)
                    throw new ArgumentException("CombatEngine attacking something with current weapon unloaded?");

                AttackRanged(attacker, attackTarget, attacker.CurrentWeapon,
                    (dmg, target, killed) => CoreGameEngine.Instance.SendTextOutput(CreateDamageString(dmg, attacker, target)));
                ((WeaponBase)attacker.CurrentWeapon).IsLoaded = false;
            }
            else
            {
                Character attackedCharacter = FindTargetAtPosition(attackTarget);
                if (attackedCharacter != null)
                {
                    int damageDone = CalculateWeaponStrike(attacker, attackedCharacter);
                    CoreGameEngine.Instance.SendTextOutput(CreateDamageString(damageDone, attacker, attackedCharacter));
                    DamageTarget(damageDone, attackedCharacter);
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
            int evadeRoll = m_random.GetRandomInt(0, 99);

            if ((bool)Preferences.Instance["ShowAttackRolls"])
            {
                CoreGameEngine.Instance.SendTextOutput(string.Format("{0} rolls to hit {1}. Rolled {2} needs above {3} to hit.",
                    attacker.Name, attackedCharacter.Name, evadeRoll, attackedCharacter.Evade));
            }

            if (evadeRoll < attackedCharacter.Evade)
                return -1;

            // Calculate damage
            float effectiveStrength = attacker.CurrentWeapon.EffectiveStrengthAtPoint(attackedCharacter.Position);
            double baseDamageDone = (int)Math.Round(attacker.CurrentWeapon.Damage.Roll() * effectiveStrength);

            // And reduce from armor
            double damageDone = ReduceDamageFromArmor(attackedCharacter, baseDamageDone);

            if ((bool)Preferences.Instance["ShowAttackRolls"])
            {
                CoreGameEngine.Instance.SendTextOutput(string.Format("Base Damage: {0}  Defense Of Target {1}  Final Damage {2}",
                    baseDamageDone, attackedCharacter.Defense, damageDone));
            }

            return (int)Math.Round(damageDone);
        }

        private double ReduceDamageFromArmor(Character attackedCharacter, double baseDamageDone)
        {
            int damageReduce = 0;
            for (int i = 0; i < attackedCharacter.Defense; ++i)
            {
                if (m_random.Chance(25))
                    damageReduce++;
            }
            return Math.Max(baseDamageDone - damageReduce, 0);
        }

        internal bool RangedBoltToLocation(Character attacker, Point target, int damageDone, object attackingMethod, DamageDoneDelegate del)
        {
            if (attacker.Position == target)
                return false;

            List<Point> attackPath = m_physicsEngine.GenerateRangedAttackListOfPoints(m_map, attacker.Position, target);

            Character targetCharacter = FindTargetAtPosition(target);
            CoreGameEngine.Instance.ShowRangedAttack(attackingMethod, ShowRangedAttackType.RangedBoltOrBlast, attackPath, targetCharacter != null);

            if (targetCharacter != null)
                DamageTarget(damageDone, targetCharacter, del);

            if (targetCharacter is Monster)
                ((Monster)targetCharacter).NoticeRangedAttack(attacker.Position);
            else
                LastTurnPlayerWasRangedAttacked = CoreGameEngine.Instance.TurnCount;

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

        public void DamageTarget(int damage, Character target)
        {
            DamageTarget(damage, target, null);
        }

        public void DamageTarget(int damage, Character target, DamageDoneDelegate del)
        {
            // Sometimes bouncy spells and other things can hit a creature two or more times.
            // If the creature is dead and the map agrees, return early, since the poor sob is already dead and gone.
            if (target.CurrentHP <= 0 && !m_map.Monsters.Contains(target))
                return;

            // -1 damage is coded for a miss
            if (damage == -1)
                return;

            target.CurrentHP -= damage;
            bool targetKilled = target.CurrentHP <= 0;
            if (targetKilled)
            {
                if (target is Monster)
                {
                    m_map.RemoveMonster(target as Monster);
                }
                else if (target is Player)
                {
                    CoreGameEngine.Instance.PlayerDied();
                }
            }
            if (del != null)
                del(damage, target, targetKilled);
        }

        private string CreateDamageString(int damage, Character attacker, Character defender)
        {
            // "Cheat" to see if attacker or defense is the player to make text output 
            // what is expected. The's should prepend monsters, not player. 
            // If we have 'Proper' named monsters, like say Kyle the Dragon, this will have to be updated.
            bool attackerIsPlayer = attacker is Player;
            bool defenderIsPlayer = defender is Player;
            bool attackKillsTarget = defender.CurrentHP <= damage;

            string verb = ((WeaponBase)attacker.CurrentWeapon).AttackVerb;

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
