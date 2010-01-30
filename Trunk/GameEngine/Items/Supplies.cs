using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Supplies : ItemWithEffects
    {
        internal Supplies(string name, string effectType, string targettingType, int strength, string itemDescription, string flavorText)
            : base(name, effectType, targettingType, strength, itemDescription, flavorText)
        {
        }

        public override string OnUseString
        {
            get
            {
                return "{0} uses the {1}.";
            }
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public override List<Interfaces.ItemOptions> PlayerOptions
        {
            get 
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Use", true),
                    new ItemOptions("Drop", true)
                };
            }
        }
    }
}
