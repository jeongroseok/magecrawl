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
                List<ItemOptions> returnList = new List<ItemOptions>();
                if (!CoreGameEngine.Instance.DangerPlayerInLOS())
                    returnList.Add(new ItemOptions("Use", true));
                returnList.Add(new ItemOptions("Drop", true));
                return returnList;
            }
        }
    }
}
