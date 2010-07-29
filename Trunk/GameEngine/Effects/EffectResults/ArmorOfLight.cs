using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    class ArmorOfLight : EffectResult
    {
        public ArmorOfLight()
        {
        }

        public ArmorOfLight(int strength, Character caster)
        {
        }

        internal override string Name
        {
            get
            {
                return "Armor Of Light";
            }
        }

        public override string GetAttribute(string key)
        {
            if (key == "DamageReduction")
                return "3";
            throw new KeyNotFoundException();
        }

        public override bool ContainsKey(string key)
        {
            return key == "DamageReduction";
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
                return (new DiceRoll(1, 11, 14)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //15-25 turns
            }
        }

        internal override int DefaultMPSustainingCost
        {
            get
            {
                return 35;
            }
        }
    }
}
