using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.Magic;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Supplies : ItemWithEffects
    {
        internal Supplies(string name, Spell spell, int strength, string itemDescription, string flavorText)
            : base(name, spell, strength, itemDescription, flavorText)
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
                List<ItemOptions> returnList = new List<ItemOptions>();
                if (!CoreGameEngine.Instance.DangerPlayerInLOS())
                    returnList.Add(new ItemOptions("Use", true));
                returnList.Add(new ItemOptions("Drop", true));
                return returnList;
            }
        }
    }
}
