using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Actors;
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

        internal void GameLoaded(Player player, Map map)
        {
            m_player = player;
            m_map = map;        
        }

        internal bool Attack(Character attacker, Direction direction)
        {
            bool didAnything = false;
            Point attackTarget = PointDirectionUtils.ConvertDirectionToDestinationPoint(attacker.Position, direction);
            foreach (Monster m in m_map.Monsters)
            {
                if (m.Position == attackTarget)
                {
                    int damageDone = 1;
                    PublicGameEngine.SendTextOutput(CreateDamageString(damageDone, attacker, m));
                    m.CurrentHP -= damageDone;
                    if (m.CurrentHP <= 0)
                        m_map.KillMonster(m);
                    didAnything = true;
                    break;
                }
            }
            if (!didAnything && attackTarget == m_player.Position)
            {
                int damageDone = 1;
                PublicGameEngine.SendTextOutput(CreateDamageString(damageDone, attacker, m_player));
                m_player.CurrentHP -= damageDone;
                if (m_player.CurrentHP <= 0)
                    CoreGameEngine.PlayerDied();
                didAnything = true;
            }
            return didAnything;
        }

        private string CreateDamageString(int dmg, Character attacker, Character defender)
        {
            // "Cheat" to see if attacker or defense is the player to make text output 
            // what is expected. The's should prepend monsters, not player. 
            // If we have 'Proper' named monsters, like say Kyle the Dragon, this will have to be updated.
            bool attackerIsPlayer = attacker is Player;
            bool defenderIsPlayer = defender is Player;
            bool attackKillsTarget = defender.CurrentHP <= dmg;

            if (dmg == -1)
            {
                if (attackerIsPlayer)
                    return string.Format("{0} misses the {1}.", attacker.Name, defender.Name);
                else if (defenderIsPlayer)
                    return string.Format("The {0} misses {1}.", attacker.Name, m_player.Name);
                else
                    return string.Format("The {0} misses the {1}.", attacker.Name, defender.Name);
            }
            else if (dmg == 0)
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
                    return string.Format("{0} strikes and kills the {1} with {2} damage.", attacker.Name, defender.Name, dmg.ToString());
                else if (defenderIsPlayer)
                    return string.Format("The {0} strikes and kills {1} with {2} damage.", attacker.Name, defender.Name, dmg.ToString());
                else
                    return string.Format("The {0} strikes and kills the {1} with {2} damage.", attacker.Name, defender.Name, dmg.ToString());
            }
            else
            {
                if (attackerIsPlayer)
                    return string.Format("{0} strikes the {1} for {2} damage.", attacker.Name, defender.Name, dmg.ToString());
                else if (defenderIsPlayer)
                    return string.Format("The {0} strikes {1} for {2} damage.", attacker.Name, defender.Name, dmg.ToString());
                else
                    return string.Format("The {0} strikes the {1} for {2} damage.", attacker.Name, defender.Name, dmg.ToString());
            }
        }
    }
}
