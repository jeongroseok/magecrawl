using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Armor
{
    internal class Headpiece : ArmorBase
    {
        public Headpiece(string name, string description, string flavorText) : base(name, description, flavorText)
        {
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                List<ItemOptions> optionList = new List<ItemOptions>();

                if (CoreGameEngine.Instance.Player.Headpiece == this)
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
