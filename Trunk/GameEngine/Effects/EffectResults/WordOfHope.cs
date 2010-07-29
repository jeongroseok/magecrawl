using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    class WordOfHope : EffectResult
    {
        public WordOfHope()
        {
        }

        public WordOfHope(int strength, Character caster)
        {
        }

        internal override string Name
        {
            get
            {
                return "Hope";
            }
        }

        public override string GetAttribute(string key)
        {
            if (key == "BonusWeaponDamage")
                return "2";
            else if (key == "BonusStamina")
                return "60";
            throw new KeyNotFoundException();
        }

        public override bool ContainsKey(string key)
        {
            return key == "BonusWeaponDamage" || key == "BonusStamina";
        }

        internal override bool IsPositiveEffect
        {
            get
            {
                return true;
            }
        }

        internal override int DefaultEffectLength
        {
            get
            {
                return (new DiceRoll(1, 7, 7)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //8-15 turns
            }
        }
    }
}
