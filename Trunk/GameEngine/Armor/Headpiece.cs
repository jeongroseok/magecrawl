using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Armor
{
    internal class Headpiece : ArmorBase
    {
        public Headpiece(string name, ArmorWeight weight, double defense, double evade, string description, string flavorText)
            : base(name, weight, defense, evade, description, flavorText)
        {
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                return PlayerOptionsInternal(CoreGameEngine.Instance.Player.Headpiece);
            }
        }
    }
}
