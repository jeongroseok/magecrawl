using Magecrawl.GameEngine.Magic;

namespace Magecrawl.GameEngine.Items
{
    internal abstract class ItemWithEffects : Item
    {
        internal ItemWithEffects(string name, Spell spell, int strength, string itemDescription, string flavorText)
            : base(name, itemDescription, flavorText)
        {
            Name = name;
            Spell = spell;
            Strength = strength;
        }

        internal string Name
        {
            get;
            private set;
        }

        internal Spell Spell
        {
            get;
            private set;
        }

        internal int Strength
        {
            get;
            private set;
        }

        override public string ItemEffectSchool
        {
            get
            {
                return Spell.School;
            }
        }

        abstract public string OnUseString
        {
            get;
        }
    }
}
