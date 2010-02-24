using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Scroll : ItemWithEffects
    {
        internal Scroll(string name, string effectType, int strength, string spellSchool, string itemDescription, string flavorText)
            : base(name, effectType, strength, itemDescription, flavorText)
        {
            ItemSchool = spellSchool;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string OnUseString
        {
            get
            {
                return "{0} reads the {1} and it turns into dust.";
            }
        }

        public override List<Magecrawl.GameEngine.Interfaces.ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Read", true),
                    new ItemOptions("Drop", true)
                };
            }
        }
    }
}
