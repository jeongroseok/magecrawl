using System.Collections.Generic;
using System.Globalization;
using Magecrawl.Interfaces;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal class Sling : RangedWeaponRangeBase, IWeaponVerb
    {
        public string AttackVerb
        {
            get
            {
                return "slings a stone at";
            }
        }
        
        public override string Name
        {
            get
            {
                return "Sling";
            }
        }
    }
}
