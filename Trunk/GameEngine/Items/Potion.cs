using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Potion : Item
    {
        private string m_effectType;
        private int m_strength;

        internal Potion(string name, string effectType, int strength, string itemDescription, string flavorText) : base(name, itemDescription, flavorText)
        {
            m_effectType = effectType;
            m_strength = strength;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public string Name
        {
            get
            {
                return DisplayName;
            }
        }

        public string EffectType
        {
            get
            {
                return m_effectType;
            }
        }

        public int Strength
        {
            get
            {
                return m_strength;
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
