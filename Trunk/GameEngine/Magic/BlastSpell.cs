using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal class BlastSpell : SpellBase
    {
        private int damage = 0;

        internal override void Cast(Character caster, CombatEngine combatEngine)
        {
            damage = new DiceRoll(1, 3, 0).Roll();
            short range = new DiceRoll(1, 4, 0).Roll();
            var toDamage = CoreGameEngine.Instance.Map.Monsters.Where(monster => PointDirectionUtils.LatticeDistance(monster.Position, CoreGameEngine.Instance.Player.Position) <= range);
            foreach (ICharacter m in toDamage)
            {
                combatEngine.Attack(caster as Character, m.Position, this);
            }
        }

        internal override int Damage
        {
            get
            {
                return damage;
            }
        }

        internal override int MagicCost
        {
            get
            {
                return 3;
            }
        }
    }
}
