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

        internal override void Cast(Character caster, CoreGameEngine engine, CombatEngine combatEngine)
        {
            damage = new DiceRoll(1, 3, 0).Roll();
            short range = new DiceRoll(1, 4, 0).Roll();
            var toDamage = engine.Map.Monsters.Where(monster => PointDirectionUtils.LatticeDistance(monster.Position, engine.Player.Position) <= range);
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
    }
}
