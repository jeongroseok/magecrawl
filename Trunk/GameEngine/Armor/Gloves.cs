using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.Armor
{
    internal class Gloves : ArmorBase
    {
        public Gloves(string name, ArmorWeight weight, int staminaBonus, double evade, string description, string flavorText)
            : base(name, weight, staminaBonus, evade, description, flavorText)
        {
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                return PlayerOptionsInternal(CoreGameEngine.Instance.Player.Gloves);
            }
        }
    }
}
