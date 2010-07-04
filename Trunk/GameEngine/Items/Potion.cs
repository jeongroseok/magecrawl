using System;
using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.Magic;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Potion : ItemWithEffects
    {
        internal Potion(string name, Spell spell, int strength, string itemDescription, string flavorText)
            : base(name, spell, strength, itemDescription, flavorText)
        { 
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string OnUseString
        {
            get
            {
                return "{0} drinks the {1}.";
            }
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Drink", true),
                    new ItemOptions("Drop", true)
                };
            }
        }
    }
}
