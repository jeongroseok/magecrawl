using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Armor
{
    internal class ChestArmor : ArmorBase
    {
        public ChestArmor(string name, ArmorWeight weight, string description, string flavorText)
            : base(name, weight, description, flavorText)
        {
        }


        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                List<ItemOptions> optionList = new List<ItemOptions>();

                if (CoreGameEngine.Instance.Player.ChestArmor == this)
                    optionList.Add(new ItemOptions("Unequip", true));
                else
                {
                    optionList.Add(new ItemOptions("Equip", true));
                    optionList.Add(new ItemOptions("Drop", true));
                }

                return optionList;
            }
        }
    }
}
