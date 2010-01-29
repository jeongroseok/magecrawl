using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Armor
{
    internal class Boots : ArmorBase
    {
        public Boots(string name, ArmorWeight weight, double defense, double evade, string description, string flavorText)
            : base(name, weight, defense, evade, description, flavorText)
        {
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                List<ItemOptions> optionList = new List<ItemOptions>();

                if (CoreGameEngine.Instance.Player.Boots == this)
                {
                    if (!CanNotUnequip)
                        optionList.Add(new ItemOptions("Unequip", true));
                }
                else
                {
                    if (IsUnequipable(CoreGameEngine.Instance.Player.Gloves))
                        optionList.Add(new ItemOptions("Equip", true));
                    optionList.Add(new ItemOptions("Drop", true));
                }

                return optionList;
            }
        }
    }
}
