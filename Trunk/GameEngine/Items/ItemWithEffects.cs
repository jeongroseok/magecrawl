namespace Magecrawl.GameEngine.Items
{
    internal abstract class ItemWithEffects : Item
    {
        internal ItemWithEffects(string name, string effectType, int strength, string itemDescription, string flavorText)
            : base(name, itemDescription, flavorText)
        {
            Name = name;
            EffectType = effectType;
            Strength = strength;
        }

        internal string Name
        {
            get;
            private set;
        }

        internal string EffectType
        {
            get;
            private set;
        }

        internal int Strength
        {
            get;
            private set;
        }

        abstract public string OnUseString
        {
            get;
        }
    }
}
