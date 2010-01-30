﻿using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Potion : ItemWithEffects
    {
        internal Potion(string name, string effectType, string targettingType, int strength, string itemDescription, string flavorText)
            : base(name, effectType, targettingType, strength, itemDescription, flavorText)
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

        public override List<Magecrawl.GameEngine.Interfaces.ItemOptions> PlayerOptions
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
