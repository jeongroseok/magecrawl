using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    // The regeneration effect doesn't do anything directly. PhyricsEngine checks for it in AfterPlayerAction()
    // If it exists, then we can heal health damage in addition to stamina damage.
    internal class Regeneration : EffectResult
    {
        public Regeneration()
        {
        }

        public Regeneration(int strength, Character caster)
        {
        }

        internal override string Name
        {
            get
            {
                return "Regen";
            }
        }

        // Needs to match class name
        internal override string Type
        {
            get
            {
                return "Regeneration";
            }
        }

        public override string GetAttribute(string key)
        {
            if (key == "Regeneration")
                return "True";
            throw new KeyNotFoundException();
        }

        public override bool ContainsKey(string key)
        {
            return key == "Regeneration";
        }

        internal override bool IsPositiveEffect
        {
            get
            {
                return true;
            }
        }

        internal override int DefaultMPSustainingCost
        {
            get
            {
                return 15;
            }
        }

        internal override int DefaultEffectLength
        {
            get
            {
                return (new DiceRoll(1, 5, 10)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //10-15 turns
            }
        }
    }
}
