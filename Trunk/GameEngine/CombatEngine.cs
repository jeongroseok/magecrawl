using System;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Level;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class CombatEngine : IDisposable
    {
        // Cared and fed by CoreGameEngine, local copy for convenience
        private Player m_player;
        private Map m_map;

        private TCODRandom m_random;

        internal CombatEngine(Player player, Map map)
        {
            m_player = player;
            m_map = map;
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

            float effectiveStrength = attacker.CurrentWeapon.EffectiveStrengthAtPoint(attackTarget);
            int damageDone = (int)Math.Round(attacker.CurrentWeapon.Damage.Roll() * effectiveStrength);

            Character attackedCharacter = FindTargetAtPosition(attackTarget);
            if (attackedCharacter != null)
            {
                CoreGameEngine.Instance.SendTextOutput(CreateDamageString(damageDone, attacker, attackedCharacter));
                DamageTarget(damageDone, attackedCharacter, null);
                return true;
            }
            return false;
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

        public delegate void DamageDoneDelegate(int damage, Character target, bool targetKilled);

        public void DamageTarget(int damage, Character target, DamageDoneDelegate del)
        {
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
                    return string.Format("{0} strikes and does no damage to the {1}.", attacker.Name, defender.Name);
                else if (defenderIsPlayer)
                    return string.Format("The {0} strikes and does no damage to {1}.", attacker.Name, defender.Name);
                else
                    return string.Format("The {0} strikes and does no damage to the {1}.", attacker.Name, defender.Name);
            }
            else if (attackKillsTarget)
            {
                if (attackerIsPlayer)
                    return string.Format("{0} strikes and kills the {1} with {2} damage.", attacker.Name, defender.Name, damage.ToString());
                else if (defenderIsPlayer)
                    return string.Format("The {0} strikes and kills {1} with {2} damage.", attacker.Name, defender.Name, damage.ToString());
                else
                    return string.Format("The {0} strikes and kills the {1} with {2} damage.", attacker.Name, defender.Name, damage.ToString());
            }
            else
            {
                if (attackerIsPlayer)
                    return string.Format("{0} strikes the {1} for {2} damage.", attacker.Name, defender.Name, damage.ToString());
                else if (defenderIsPlayer)
                    return string.Format("The {0} strikes {1} for {2} damage.", attacker.Name, defender.Name, damage.ToString());
                else
                    return string.Format("The {0} strikes the {1} for {2} damage.", attacker.Name, defender.Name, damage.ToString());
            }
        }
    }
}
