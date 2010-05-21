using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Armor
{
    internal class ChestArmor : ArmorBase
    {
        public ChestArmor(string name, ArmorWeight weight, int staminaBonus, double evade, string description, string flavorText)
            : base(name, weight, staminaBonus, evade, description, flavorText)
        {
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                return PlayerOptionsInternal(CoreGameEngine.Instance.Player.ChestArmor);
            }
        }
    }
}
